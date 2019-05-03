@echo off
title Creating PKG files
set tmp=
set src=
set fsz=
set sfo=
set fdr=
set fnm=
set dnm=
set chk=%fnm%\chunk-*.txt
set col="%tmp%\nhcolor"
if not exist "%chk%" (
echo.
echo The game has a file of more than %fsz% byte. This folder can not be divided into parts.
echo Exiting...
echo.
pause
goto exit
)
SetLocal EnableExtensions EnableDelayedExpansion
:: ???????? ContentID ?? EBOOT.BIN, ???? ????????????
if exist "!fdr!\USRDIR\EBOOT.BIN" (
"!tmp!\sfk" hexdump -pure -nofile -rawname -offlen 0x450 0x24 "!fdr!\USRDIR\EBOOT.BIN" +hextobin "!tmp!\cid.txt" > nul
for /f "usebackq" %%I in ("!tmp!\cid.txt") do set cid=%%I
set cidend=!cid:~20,14!
)
:: ???????? ContentID ?? ISO.BIN.EDAT, ???? ????????????
if exist "!fdr!\USRDIR\ISO.BIN.EDAT" (
"!tmp!\sfk" hexdump -pure -nofile -rawname -offlen 0x10 0x24 "!fdr!\USRDIR\ISO.BIN.EDAT" +hextobin "!tmp!\cid.txt" > nul
for /f "usebackq" %%I in ("!tmp!\cid.txt") do set cid=%%I
set cidend=!cid:~20,14!
)
:: ??????? PARAM.SFO, ICON0.PNG ? EBOOT.BIN ?? ????????? ?????
if exist "!sfo!" xcopy /y "!sfo!" "!tmp!" > nul
if exist "!fdr!\ICON0.PNG" xcopy /y "!fdr!\ICON0.PNG" "!tmp!" > nul
if exist "!fdr!\USRDIR\EBOOT.BIN" xcopy /y "!fdr!\USRDIR\EBOOT.BIN" "!tmp!" > nul
:: ???????? ??? ????? ? ??? ????? ? ??????? ??????? ?? 4 ??
for %%A in ("!chk!") do (
set fn=%%~nA
set ch=!fn:~6,2!
:: ???????? ????? ?? ?????? ?? ??????? ????? ? ???? ????? ? ????????? ???????? ?????
echo Preparing folder for the parts : !fn!|!col! 0E
echo.
echo Moving the !fn! files to the SPLITTED folder. Please wait...|!col! 02
for /f "usebackq tokens=* delims=" %%B in ("%%A") do (
attrib -r "!fnm!\%%~B"
set pn=%%B
xcopy "!fnm!\%%~B" "!fnm!\SPLITTED\!fn!\!pn:%%~nxB=!" /t /e > nul
move "!fnm!\%%~B" "!fnm!\SPLITTED\!fn!\!pn:%%~nxB=!" > nul
)
if exist "!tmp!\PARAM.SFO" xcopy /y "!tmp!\PARAM.SFO" "!fnm!\SPLITTED\!fn!\!dnm!" > nul
if exist "!tmp!\ICON0.PNG" xcopy /y "!tmp!\ICON0.PNG" "!fnm!\SPLITTED\!fn!\!dnm!" > nul
if exist "!tmp!\EBOOT.BIN" xcopy /y "!tmp!\EBOOT.BIN" "!fnm!\SPLITTED\!fn!\!dnm!\USRDIR" > nul

:exit
