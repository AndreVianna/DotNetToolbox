@echo off

if [%1]==[] goto USAGE
set target=%1

cd ..\..
call Publish %target% Applications\Console Console 9.0.0-alpha1
goto :eof

:USAGE
echo Usage:
echo Publish ^<local^|remote^>
echo;

