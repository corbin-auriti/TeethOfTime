@echo off
cls
echo -=Teeth Of Time batch launcher=-
echo.
echo Enter path to the world you want to ruin.
echo The world has to be in the Teeth Of Time folder or in the Appdata folder.
echo.
echo List of worlds in your Appdata:
goto :list
:set
SET /P worldName=Enter name of the world to be aged: 
SET worldName=%worldName:"=%
if exist "%cd%\%worldName%\level.dat" ( goto :prerun )
if exist "%appdata%\.minecraft\saves\%worldName%\level.dat" ( goto :prerun2 )
echo %worldName% does not exist!
goto :set

:prerun
goto :list21

:prerun2
goto :list22

:prerun12
SET /P cfgName=Enter name of the config to be used: 
SET cfgName=%cfgName:"=%
GOTO:run

:prerun22
SET /P cfgName=Enter name of the config to be used: 
SET cfgName=%cfgName:"=%
GOTO:run2

:list
echo.
dir /b /ad "%appdata%\.minecraft\saves\*"
echo.
goto :set

:list21
echo.
echo List of configs in Configs directory:
echo.
dir /b "%cd%\configs\*.cfg"
echo.
goto :prerun21

:list22
echo.
echo List of configs in Configs directory:
echo.
dir /b "%cd%\configs\*.cfg"
echo.
goto :prerun22

:run
@echo on
TeethOfTime.exe "%worldName%" "%cfgName%"
@echo off
pause
GOTO:EOF

:run2
@echo on
TeethOfTime.exe "%appdata%\.minecraft\saves\%worldName%" "%cfgName%"
@echo off
pause
GOTO:EOF