cls
$package = (Get-ChildItem .\bin\Release -Filter *.nupkg | Sort-Object LastWriteTime | Select-Object -Last 1)
$source = "https://api.nuget.org/v3/index.json"

if(!$package){
    Write-Error "*.nupkg package not found." -Category ResourceUnavailable
} else {
    Write-Host ""
    Write-Host "NOTE: if you missing APIKEY, then run this command: nuget setapikey %apikey% -Source $source"
    Write-Host ""

    <# Push package to nuget #>
    nuget push $package.FullName -Source $source
}

<#Prevent from closing #>
Write-Host ""
Read-Host -Prompt "Press Enter to exit"