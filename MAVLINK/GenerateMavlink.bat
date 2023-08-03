rd /s /q C
::/s删除目录和子目录，/q安静模式
rd /s /q CS
xcopy pymavlink\CS CS /s /i /y
::复制指定目录和子目录至目标目录，/i如果目标目录不存在则创建，/y不需要确认
for %%i in (*.xml) do (
	python -m pymavlink.tools.mavgen --lang=C --wire-protocol=1.0 --output=.\C %%i
	python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=1.0 --output=.\CS\ %%i
)
@echo off&setlocal enabledelayedexpansion
for %%i in (*.txt) do (
ren %%i 版本日期%date:~0,4%年%date:~5,2%月%date:~8,2%日.txt
)
pause
