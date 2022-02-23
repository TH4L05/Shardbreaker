@echo off

SETLOCAL

IF EXIST ..\..\WwiseRoot.txt (
	FOR /F %%x in (..\..\WwiseRoot.txt) DO (
		SET ADDITIONAL=--wwiseroot=%%x
		SET "WWISEROOT=%%x"
		CALL SET "WWISEROOT=%%WWISEROOT:/=\%%"
		GOTO :RUN
	)
)

:RUN

call "%WWISEROOT%\Tools\Win32\bin\premake5" --scripts="%WWISEROOT%\Scripts\Premake" %ADDITIONAL% %*

ENDLOCAL