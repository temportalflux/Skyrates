
SET buildDir=%1
SET targetDir=%2
SET exeName=%3
SET dllName=%4

SET buildExe=%buildDir%%exeName%
SET buildDll=%buildDir%%dllName%
SET targetExe=%targetDir%%exeName%
SET targetDll=%targetDir%%dllName%

ECHO F|xcopy /E /Y /R %buildExe% %targetExe%
ECHO F|xcopy /E /Y /R %buildDll% %targetDll%
