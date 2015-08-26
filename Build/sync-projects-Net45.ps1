$path = Split-Path -Path $MyInvocation.MyCommand.Path
$syncProjectsPath = $path + "\sync-projects.ps1"

# Normal projects
$projectsPath = "'" + $path + "\..\src\Mixpanel\Mixpanel" + "'"
$projectsParams = @(
  "-projectsPath", $projectsPath, 
  "-sourceProject", "Mixpanel.csproj")
Invoke-Expression "& `"$syncProjectsPath`" $projectsParams"

# Test projects
$testProjectsPath = "'" + $path + "\..\src\Mixpanel\Mixpanel.Tests" + "'"
$testProjectsParams = @(
  "-projectsPath", $testProjectsPath, 
  "-sourceProject", "Mixpanel.Tests.csproj")
Invoke-Expression "& `"$syncProjectsPath`" $testProjectsParams"