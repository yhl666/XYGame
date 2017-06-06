 
@echo off
taskkill /f /t /im BattleServer.exe
taskkill /f /t /im ClientServer.exe

 

tasklist|find /i "redis-server.exe"
if ERRORLEVEL 1  start redis.bat

cd ClientServer
start ClientServer.exe -batchmode
cd ..

cd BattleServer/Release
start  BattleServer.exe
cd ..
cd ..

cd LogicServer\server\bin
rerun.bat
cd ..
cd ..
cd ..

exit