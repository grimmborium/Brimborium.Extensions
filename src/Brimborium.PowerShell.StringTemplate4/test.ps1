# cd /D G:\github\grimmborium\Brimborium.Extensions\src\Brimborium.PowerShell.StringTemplate4
# pwsh
# G:\github\grimmborium\Brimborium.Extensions\src\Brimborium.PowerShell.StringTemplate4\bin\Debug\netstandard2.0\Brimborium.PowerShell.StringTemplate4.dll
Import-Module "G:\github\grimmborium\Brimborium.Extensions\src\Brimborium.PowerShell.StringTemplate4\bin\Debug\netstandard2.0\Brimborium.PowerShell.StringTemplate4.psd1" -verbose
Import-Module "G:\github\grimmborium\Brimborium.Extensions\src\Brimborium.PowerShell.StringTemplate4\bin\Debug\netstandard2.0\Brimborium.PowerShell.StringTemplate4.dll" -verbose

Save-ST4TemplateOutput -BasePath G:\github\grimmborium\

$tgf = New-ST4TemplateGroupFile -FilePath .\test1.stg