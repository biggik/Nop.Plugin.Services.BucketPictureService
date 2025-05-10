@echo off
set plugin=Misc.BucketPictureService
if "%~1" == "" goto MissingParam
if "%~2" == "" goto MissingParam
set nopVersion=%1
set configuration=%2
echo Running custom build step for %plugin% (%configuration%) and %nopVersion%

echo %cd%
echo Cleaning up old build files ...
rem Cleanup local folders
del ..\_build\%configuration%\nop.core.* /q > nul
del ..\_build\%configuration%\nop.data.* /q > nul
del ..\_build\%configuration%\nop.web.* /q > nul
del ..\_build\%configuration%\nop.services.* /q > nul
rd ..\_build\%configuration%\ref /s /q > nul
rd ..\_build\%configuration%\refs /s /q > nul
rd ..\_build\%configuration%\runtimes /s /q > nul
echo ... cleanup done

if "%~3" NEQ "" copy %3 ..\_build\%configuration%\. /y
if "%~4" NEQ "" copy %4 ..\_build\%configuration%\. /y
if "%~5" NEQ "" copy %5 ..\_build\%configuration%\. /y
if "%~6" NEQ "" copy %6 ..\_build\%configuration%\. /y
if "%~7" NEQ "" copy %7 ..\_build\%configuration%\. /y

rem Copy to nopCommerce src folder
if "%configuration%" == "release" goto ReleaseBuild

set nopOut="n:\nopCommerce %nopVersion%\Presentation\Nop.Web\Plugins\%plugin%"
if not exist %nopOut% (
    mkdir %nopOut%
    echo Created %nopOut%
)
if exist ..\_build\%configuration%\Views (
    if not exist %nopOut%\Views (
        mkdir %nopOut%\Views
        echo Created %nopOut%Views
    )
)
if exist ..\_build\%configuration%\Content (
    if not exist %nopOut%\Content (
        mkdir %nopOut%\Content
        echo Created %nopOut%Content
    )
)
if exist ..\_build\%configuration%\logo.jpg (
    Echo Copying logo
    xcopy /d /y /Q "..\_build\%configuration%\logo.jpg" %nopOut%\. > nul
)
if exist ..\_build\%configuration%\plugin.json (
    echo Copying plugin.json
    xcopy /d /y /Q "..\_build\%configuration%\plugin.json" %nopOut%\. > nul
)
if exist ..\_build\%configuration%\Nop.Plugin.%plugin%*.* (
    echo Copying Plugin files
    xcopy /d /y /Q "..\_build\%configuration%\Nop.Plugin.%plugin%*.*" %nopOut%\. > nul
)
if exist ..\_build\%configuration%\nopLocalizationHelper.dll (
    echo Copying localizer
    xcopy /d /y /Q "..\_build\%configuration%\nopLocalizationHelper.dll" %nopOut%\. > nul
)
if exist ..\_build\%configuration%\Views (
    echo Copying views
    xcopy /d /y /Q /S "..\_build\%configuration%\Views\." %nopOut%\Views\. > nul
)
IF %ERRORLEVEL% NEQ 0 ( 
    echo Errorlevel is %ErrorLevel%
)
echo %nopOut% was updated
goto AllDone
:MissingParam
echo This command script requires two parameters - the nopCommerce version being targeted (e.g. 4.40)
echo And the build configuration (%configuration% or release)
goto AllDone
:ReleaseBuild
echo Nothing done, this is a release build
:AllDone
