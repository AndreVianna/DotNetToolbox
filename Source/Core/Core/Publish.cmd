@echo off

if [%1]==[] goto USAGE
set target=%1

cd ..\..
call Publish %target% Core\Core Core 9.0.0
goto :eof

:USAGE
echo Usage:
echo Publish ^<local^|remote^>
echo;

