using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Mixpanel")]
[assembly: AssemblyDescription("Mixpanel .NET integration library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Aleksandr Ivanov")]
[assembly: AssemblyProduct("Mixpanel")]
[assembly: AssemblyCopyright("Copyright © 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if !(PORTABLE || PORTABLE40)
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f2d0c0d5-4180-4b01-bf62-91bdffb408c5")]
#endif


[assembly: InternalsVisibleTo("Mixpanel.Tests")]

// Version information for an assembly consists of the following four values:
//
// MAJOR version when you make incompatible API changes,
// MINOR version when you add functionality in a backwards-compatible manner, and
// PATCH version when you make backwards-compatible bug fixes.
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.0.0")]
