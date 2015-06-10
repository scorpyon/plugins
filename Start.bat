@echo off
echo Leave window open to auto restart after a crash.
echo:
:Loop
ROK.exe -batchmode -nographics
goto loop
