function Get-SolutionConfigurations($solution)
{
        Get-Content $solution |
        Where-Object {$_ -match "(?<config>\w+)\|"} |
        %{ $($Matches['config'])} |
        select -uniq
}


$buildScriptPath=$PSScriptRoot
$projectName="Specflow-Browserstack"
$solutionPath="$buildScriptPath\..\$projectName.sln"
$csprojPath="$buildScriptPath\..\$projectName"
$csproj="$csprojPath\$projectName.csproj"
$msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
$nunit="$buildScriptPath\..\packages\NUnit.Console.3.0.0\tools\nunit3-console.exe"


Write-Host "Build"
@(Get-SolutionConfigurations $solutionPath) | foreach {
	Write-Host " - $_"
    & $msbuild $csproj /p:Configuration=$_ /nologo /verbosity:quiet
}


Write-Host "Run Tests"
@(Get-SolutionConfigurations $solutionPath)| foreach {
    Start-Job -Name $configuration -ScriptBlock {
        param($configuration)
 
        $nunit --result:nunit_$configuration.xml --config:$configuration $csprojPath\bin\$configuration\$projectName.dll
    } -ArgumentList $_ 
}
Get-Job | Wait-Job
Get-Job | Receive-Job