rd /s /q C
::/sɾ��Ŀ¼����Ŀ¼��/q����ģʽ
rd /s /q CS
xcopy pymavlink\CS CS /s /i /y
::����ָ��Ŀ¼����Ŀ¼��Ŀ��Ŀ¼��/i���Ŀ��Ŀ¼�������򴴽���/y����Ҫȷ��
for %%i in (*.xml) do (
	python -m pymavlink.tools.mavgen --lang=C --wire-protocol=1.0 --output=.\C %%i
	python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=1.0 --output=.\CS\ %%i
)
@echo off&setlocal enabledelayedexpansion
for %%i in (*.txt) do (
ren %%i �汾����%date:~0,4%��%date:~5,2%��%date:~8,2%��.txt
)
pause
