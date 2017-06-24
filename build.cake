#tool nuget:?package=NUnit.ConsoleRunner&VERSION=3.6.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

var SOLUTION_PATH = Directory("./src/Mixpanel");
var SOLUTION_FILE = SOLUTION_PATH + File("Mixpanel.sln");

var PROJ_PATH = SOLUTION_PATH + Directory("Mixpanel");
var PROJ_FILE = PROJ_PATH + File("Mixpanel.csproj");

var TESTS_PROJ_PATH = SOLUTION_PATH + Directory("Mixpanel.Tests");
var TESTS_PROJ_FILE = TESTS_PROJ_PATH + File("Mixpanel.Tests.csproj");

var BIN_DIR = Directory("./src/Mixpanel/Mixpanel/bin") + Directory(configuration);
var TESTS_BIN_DIR = Directory("./src/Mixpanel/Mixpanel.Tests/bin") + Directory(configuration);

var ASSEMBLY_VERSION =
    XmlPeek(PROJ_FILE, "/Project/PropertyGroup/AssemblyVersion/text()");
var PACKAGE_VERSION = 
    ASSEMBLY_VERSION.Substring(0, ASSEMBLY_VERSION.Length - 2) + 
    (configuration == "Debug" ? "-dbg" : "");

var IMAGE_DIR = Directory("./images") + Directory(PACKAGE_VERSION);
var PACKAGE_DIR = Directory("./package");
var ZIP_FILE = PACKAGE_DIR + File("mixpanel-csharp-" + PACKAGE_VERSION + ".zip");

///////////////////////////////////////////////////////////////////////////////
// SETUP
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information("Building Mixpanel.");
    Information("Assembly Version: " + ASSEMBLY_VERSION);
    Information("Package Version: " + PACKAGE_VERSION);

    CheckIfDotNetCoreInstalled();
});

//////////////////////////////////////////////////////////////////////
// CLEAN
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(BIN_DIR);
    CleanDirectory(TESTS_BIN_DIR);
});

//////////////////////////////////////////////////////////////////////
// BUILD
//////////////////////////////////////////////////////////////////////

Task("BuildSolution")
    .Description("Builds Mixpanel solution.")
    .Does(() =>
    {
        MSBuild(SOLUTION_FILE, settings =>
            settings
                .SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal));
       
    });

//////////////////////////////////////////////////////////////////////
// TEST
//////////////////////////////////////////////////////////////////////

Task("TestSolution")
    .Description("Tests all versions of Mixpanel.")
    .IsDependentOn("BuildSolution")
    .Does(() =>
    {  
        var testAssemblyDirs = GetDirectories(TESTS_BIN_DIR.ToString() + "/*");
        foreach(DirectoryPath testAssemblyDir in testAssemblyDirs)
        {
            string framework = testAssemblyDir.GetDirectoryName();
            string testAssemblyFile = GetTestAssemblyFile(testAssemblyDir);
            
            if(framework.Contains("netcore"))
            {
                RunDotnetCoreTest(framework);
            }
            else
            {
                RunNUnitTests(testAssemblyFile);
            }            
        }
    });

string GetTestAssemblyFile(DirectoryPath testAssemblyDir)
{
    return 
        Directory(testAssemblyDir.ToString()) + 
        File("Mixpanel.Tests.dll");
}

void RunNUnitTests(string testAssemblyFile)
{
    var settings = new NUnit3Settings();
    NUnit3(testAssemblyFile, settings);
}

void RunDotnetCoreTest(string framework)
{
    string arguments = 
        "test " + TESTS_PROJ_FILE + " -f " + framework + " -c " + configuration + " --no-build";

    int rc = StartProcess("dotnet", arguments);    
    if(rc != 0) 
    {
        throw new Exception("Error running tests for " + framework);
    }
}

//////////////////////////////////////////////////////////////////////
// NUGET PACKAGE
//////////////////////////////////////////////////////////////////////

Task("CreateImage")
    .Description("Copies all files into the image directory.")
    .Does(() =>
    {  
        CleanDirectory(IMAGE_DIR);

        var imageLibDir = IMAGE_DIR + Directory("lib");
        CreateDirectory(imageLibDir);      
        CopyDirectory(BIN_DIR, imageLibDir);         
    });

Task("Create-NuGet-Package")
    .IsDependentOn("CreateImage")
    .Does(() =>
    {
        CreateDirectory(PACKAGE_DIR);
        CleanDirectory(PACKAGE_DIR);

        NuGetPack("mixpanel-csharp.nuspec", new NuGetPackSettings()
        {
            Version = PACKAGE_VERSION,
            BasePath = IMAGE_DIR,
            OutputDirectory = PACKAGE_DIR
        });
    });

Task("PackageZip")
    .Description("Creates a ZIP file of the framework")
    .IsDependentOn("CreateImage")
    .Does(() =>
    {
        CreateDirectory(PACKAGE_DIR);
        Zip(IMAGE_DIR, File(ZIP_FILE));
    });

//////////////////////////////////////////////////////////////////////
// HELPER METHODS - GENERAL
//////////////////////////////////////////////////////////////////////

void CheckIfDotNetCoreInstalled()
{
    try
    {
        Information("Checking if .NET Core SDK is installed");
        StartProcess("dotnet", new ProcessSettings
        {
            Arguments = "--version"
        });
    }
    catch(Exception e)
    {
        Error(".NET Core SDK is not installed. It can be installed from https://www.microsoft.com/net/core");
        throw e;
    }
}


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .Description("Builds all versions of Mixpanel.")
    .IsDependentOn("BuildSolution");

Task("Test")
    .Description("Builds and tests all versions of Mixpanel.")
    .IsDependentOn("Build")
    .IsDependentOn("TestSolution");

Task("NuGet")
    .Description("Creates Mixpanel NuGet package.")
    .IsDependentOn("Create-NuGet-Package")
    .IsDependentOn("PackageZip");

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
