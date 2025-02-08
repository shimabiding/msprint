 
if Defined appRoot GOTO Post
set NOW_CD=%cd%
cd %~dp0
set HOME=%cd%
cd %NOW_CD%
set appRoot=%HOME%\apps
set gitDir=%appRoot%\mingit
set PATH=%gitDir%\cmd;%gitDir%;%gitDir%\usr\bin;%PATH%
set PATH=%appRoot%\7z2408-extra;%PATH%
set PATH=%appRoot%\vsc;%PATH%
set PATH=%appRoot%\vsc\data\user-data\User\globalStorage\ms-dotnettools.vscode-dotnet-runtime\.dotnet\8.0.10~x64~aspnetcore;%PATH%
set PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319;%PATH%

:Post
echo home has located in %HOME%
echo applications have located in %appRoot%
 