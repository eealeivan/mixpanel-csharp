properties { 
  #Given a version number MAJOR.MINOR.PATCH, increment the:
  #MAJOR version when you make incompatible API changes,
  #MINOR version when you add functionality in a backwards-compatible manner, and
  #PATCH version when you make backwards-compatible bug fixes.
  $version = "2.1.0"
  $packageId = "mixpanel-csharp"
  $signAssemblies = $false
  $signKeyPath = ""
  $treatWarningsAsErrors = $false
  $workingName = if ($workingName) {$workingName} else {"Working"}
  
  $baseDir  = resolve-path ..
  $buildDir = "$baseDir\Build"
  $sourceDir = "$baseDir\src\Mixpanel"
  $toolsDir = "$baseDir\Tools"
  $releaseDir = "$baseDir\Release"
  $workingDir = "$baseDir\$workingName"
  $builds = @(  
    @{Name = "Mixpanel"; TestsName = "Mixpanel.Tests"; Constants=""; FinalDir="Net45"; NuGetDir = "net45"; Framework="net-4.5.1"; Sign=$false},
    @{Name = "Mixpanel.Net40"; TestsName = "Mixpanel.Tests.Net40"; Constants="NET40"; FinalDir="Net40"; NuGetDir = "net40"; Framework="net-4.0"; Sign=$false},
    @{Name = "Mixpanel.Net35"; TestsName = "Mixpanel.Tests.Net35"; Constants="NET35"; FinalDir="Net35"; NuGetDir = "net35"; Framework="net-2.0"; Sign=$false},  
    @{Name = "Mixpanel.Portable"; TestsName = "Mixpanel.Tests.Portable"; Constants="PORTABLE"; FinalDir="Portable"; NuGetDir = "portable-net45+netcore45+wpa81+wp8"; Framework="net-4.5"; Sign=$false}  
  )
}

framework '4.5.1x86'

task default -depends Test

# Ensure a clean working directory
task Clean {
  Write-Host "Setting location to $baseDir"
  Set-Location $baseDir
  
  if (Test-Path -path $workingDir)
  {
    Write-Host "Deleting existing working directory $workingDir"
    
    del $workingDir -Recurse -Force
  }
  
  Write-Host "Creating working directory $workingDir"
  New-Item -Path $workingDir -ItemType Directory
}

# Build each solution
task Build -depends Clean {
  Write-Host -ForegroundColor Green "Updating assembly version"
  Write-Host
  Update-AssemblyInfoFiles $sourceDir $version
  
  foreach ($build in $builds)
  {
    $name = $build.Name
    if ($name -ne $null)
    {
      $finalDir = $build.FinalDir
      $sign = ($build.Sign -and $signAssemblies)

      Write-Host -ForegroundColor Green "Building " $name
      Write-Host -ForegroundColor Green "Signed " $sign
      Write-Host -ForegroundColor Green "Key " $signKeyPath

      Write-Host
      Write-Host "Restoring"
      [Environment]::SetEnvironmentVariable("EnableNuGetPackageRestore", "true", "Process")
      exec { .\src\Mixpanel\.nuget\NuGet.exe update -self }
      exec { .\src\Mixpanel\.nuget\NuGet.exe restore ".\src\Mixpanel\$name.sln" | Out-Default } "Error restoring $name"

      Write-Host
      Write-Host "Building"
      exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release "/p:Platform=Any CPU" /p:OutputPath=bin\Release\$finalDir\ /p:AssemblyOriginatorKeyFile=$signKeyPath "/p:SignAssembly=$sign" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" "/p:VisualStudioVersion=12.0" (GetConstants $build.Constants $sign) ".\src\Mixpanel\$name.sln" | Out-Default } "Error building $name"
    }
  }
}

# Copy files to Working directory and create NuGet package
task Package -depends Build {
  foreach ($build in $builds)
  {
    $finalDir = $build.FinalDir
    
    robocopy "$sourceDir\Mixpanel\bin\Release\$finalDir" $workingDir\Package\Bin\$finalDir *.dll *.pdb *.xml /NFL /NDL /NJS /NC /NS /NP /XO /XF *.CodeAnalysisLog.xml | Out-Default
  }
  
  New-Item -Path $workingDir\NuGet -ItemType Directory
  
  $nuspecPath = "$workingDir\NuGet\mixpanel-csharp.nuspec"
  Copy-Item -Path "$buildDir\mixpanel-csharp.nuspec" -Destination $nuspecPath -recurse

  Write-Host "Updating nuspec file at $nuspecPath" -ForegroundColor Green
  Write-Host
  
  $xml = [xml](Get-Content $nuspecPath)
  Edit-XmlNodes -doc $xml -xpath "//*[local-name() = 'id']" -value $packageId
  Edit-XmlNodes -doc $xml -xpath "//*[local-name() = 'version']" -value $version
  
  Write-Host $xml.OuterXml

  $xml.save($nuspecPath)
  
  foreach ($build in $builds)
  {
    if ($build.NuGetDir)
    {
      $finalDir = $build.FinalDir
      $frameworkDirs = $build.NuGetDir.Split(",")
      
      foreach ($frameworkDir in $frameworkDirs)
      {
        robocopy "$sourceDir\Mixpanel\bin\Release\$finalDir" $workingDir\NuGet\lib\$frameworkDir *.dll *.pdb *.xml /NFL /NDL /NJS /NC /NS /NP /XO /XF *.CodeAnalysisLog.xml | Out-Default
      }
    }
  }
  
  Write-Host "Building NuGet package with ID $packageId and version $version" -ForegroundColor Green
  Write-Host

  exec { .\src\Mixpanel\.nuget\NuGet.exe pack $nuspecPath -Symbols }
  move -Path .\*.nupkg -Destination $workingDir\NuGet
}

# Run tests on deployed files
task Test -depends Package {
  foreach ($build in $builds)
  {
    NUnitTests $build
  }
}

function NUnitTests($build)
{
  $name = $build.TestsName
  $finalDir = $build.FinalDir
  $framework = $build.Framework

  Write-Host -ForegroundColor Green "Copying test assembly $name to deployed directory"
  Write-Host
  robocopy ".\src\Mixpanel\Mixpanel.Tests\bin\Release\$finalDir" $workingDir\Deployed\Bin\$finalDir /MIR /NP /XO | Out-Default

  Copy-Item -Path ".\src\Mixpanel\Mixpanel.Tests\bin\Release\$finalDir\Mixpanel.Tests.dll" -Destination $workingDir\Deployed\Bin\$finalDir\

  Write-Host -ForegroundColor Green "Running NUnit tests " $name
  Write-Host
  exec { .\Tools\NUnit\nunit-console.exe "$workingDir\Deployed\Bin\$finalDir\Mixpanel.Tests.dll" /framework=$framework /xml:$workingDir\$name.xml | Out-Default } "Error running $name tests"
}

function GetConstants($constants, $includeSigned)
{
  $signed = switch($includeSigned) { $true { ";SIGNED" } default { "" } }

  return "/p:DefineConstants=`"CODE_ANALYSIS;TRACE;$constants$signed`""
}

function Edit-XmlNodes {
    param (
        [xml] $doc,
        [string] $xpath = $(throw "xpath is a required parameter"),
        [string] $value = $(throw "value is a required parameter")
    )
    
    $nodes = $doc.SelectNodes($xpath)
    $count = $nodes.Count

    Write-Host "Found $count nodes with path '$xpath'"
    
    foreach ($node in $nodes) {
        if ($node -ne $null) {
            if ($node.NodeType -eq "Element")
            {
                $node.InnerXml = $value
            }
            else
            {
                $node.Value = $value
            }
        }
    }
}

function Update-AssemblyInfoFiles ([string] $sourceDir, [string] $version)
{
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyVersion = 'AssemblyVersion("' + $version + '.0")';
    $fileVersion = 'AssemblyFileVersion("' + $version + '.0")';
    
    Get-ChildItem -Path $sourceDir -r -filter AssemblyInfo.cs | ForEach-Object {
        
        $filename = $_.Directory.ToString() + '\' + $_.Name
        Write-Host $filename
        $filename + ' -> ' + $version
    
        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Set-Content $filename -Encoding UTF8
    }
}