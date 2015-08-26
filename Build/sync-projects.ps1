Param(
  [string]$projectsPath,
  [string]$sourceProject,
  [switch]$syncReferences
)


function CreateXmlElement ([System.Xml.XmlDocument]$xml, [string]$namespace, [string]$elementName, [string]$value, [hashtable]$attributes) { 
  $xmlElement = $xml.CreateElement($elementName, $namespace) # namespace is needed to avoid adding an empty xmlns attribute   
  
  if($value) {
    $xmlElement.InnerText = $value;
  }
  
  if($attributes) {
    $attributes.GetEnumerator() | % { 
	  $xmlElement.SetAttribute($_.key, $_.value)     
    }
  }
  
  return $xmlElement  
}

Write-Host -ForegroundColor Green ("Start sync project files. " + $sourceProject + " is used as base.")

$sourceXml = [xml](Get-Content ($projectsPath + "\" + $sourceProject))

# Get all file names included in source project
$sourceCompileIncludes = $sourceXml.Project.ChildNodes.Item(5).ChildNodes

# Get all references from source project
$sourceReferences = $sourceXml.Project.ChildNodes.Item(4).ChildNodes

$targetProjects = (Get-ChildItem –Path $projectsPath | Where-Object {$_.Extension -eq ".csproj"}).Name
foreach($targetProject in $targetProjects) {
  if($targetProject -ne $sourceProject) {
    Write-Host -ForegroundColor Green ("Sync " + $targetProject)
	
	$targetProjectPath = $projectsPath + "\" + $targetProject;
	$targetXml = [xml](Get-Content $targetProjectPath)
	$targetNamespace = $targetXml.Project.xmlns	
	
	$targetCompileXml = $targetXml.Project.ChildNodes.Item(5)	
	$targetCompileXml.RemoveAll()
	foreach($sourceCompileInclude in $sourceCompileIncludes) {
	  $targetCompileIncludeXml = CreateXmlElement $targetXml $targetNamespace "Compile" $null @{"Include" = $sourceCompileInclude.Include}
	  $targetCompileXml.AppendChild($targetCompileIncludeXml)
	}
	
	if($syncReferences -and (($targetProject -notlike "*portable*") -or ($targetProject -like "*tests*"))) {
	  $targetReferenceXml = $targetXml.Project.ChildNodes.Item(4)	
	  $targetReferenceXml.RemoveAll()
	  foreach($sourceReference in $sourceReferences) {
	    $targetReferenceIncludeXml = CreateXmlElement $targetXml $targetNamespace "Reference" $null @{"Include" = $sourceReference.Include}	   
		if($sourceReference.HintPath) {
		  $targetReferenceHintPathXml = CreateXmlElement $targetXml $targetNamespace "HintPath" $sourceReference.HintPath 
		  $targetReferenceIncludeXml.AppendChild($targetReferenceHintPathXml)
		}
		
	    $targetReferenceXml.AppendChild($targetReferenceIncludeXml)
	  }
	}
	
	$targetXml.Save($targetProjectPath)
  }
}
