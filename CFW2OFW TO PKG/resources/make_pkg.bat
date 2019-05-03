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
:: ?????????? ? ?????????? ?????? ??? ???????
set conf="!tmp!\package.conf"
for /f "usebackq tokens=3" %%D in (`!tmp!\sfoprint "!tmp!\PARAM.SFO" TITLE_ID`) do set title=%%D00000!ch!
for /f "usebackq tokens=3" %%E in (`!tmp!\sfoprint "!tmp!\PARAM.SFO" CATEGORY`) do set cat=%%E
for /f "usebackq tokens=3" %%F in (`!tmp!\sfoprint "!tmp!\PARAM.SFO" APP_VER`) do set apver=%%F
if not defined apver set apver=1.00
set DRM=Free
if !cat!==HD (
set ct=Game_Exec
set pt=HDDGamePatch
set n1=1HDCAT
)
if !cat!==HG (
set ct=Game_Exec
set pt=HDDGamePatch
set n1=1HGCAT
)
if !cat!==DG (
set ct=Game_Exec
set pt=HDDGamePatch
set n1=1DGCAT
)
if !cat!==GD (
set ct=Game_Data
set pt=DiscGamePatch
set n1=2PATCH
)
if !cat!==AP (
set ct=Game_Exec
set pt=HDDGamePatch
set n1=1APCAT
set title=APPPHOTO000000!ch!
)
if !cat!==AM (
set ct=Game_Exec
set pt=HDDGamePatch
set n1=1AMCAT
set title=APPMUSIC000000!ch!
)
if !cat!==AV (
set ct=Game_Exec
set pt=HDDGamePatch
set n1=1AVCAT
set title=APPVIDEO000000!ch!
)
if !cat!==AT (
set ct=Game_Exec
set pt=HDDGamePatch
set n1=1ATCAT
set title=APPTV000000000!ch!
)
if !cat!==2G (
set EDT=ISO.BIN.EDAT
set ct=Game_Data
set n1=12GCAT
set title=PS2DISCINSTALL!ch!
)
if !cat!==2P (
set EDT=ISO.BIN.EDAT
set ct=Game_Data
set n1=12PCAT
set title=PS2CLASSICS000!ch!
)
if !cat!==2D (
set EDT=ISO.BIN.EDAT
set ct=Game_Data
set n1=22DCAT
set title=PS2EMUDATA0000!ch!
)
if !cat!==1P (
set EDT=ISO.BIN.EDAT
set ct=Game_Data
set n1=11PCAT
set title=PS1CLASSICS000!ch!
)
if !cat!==MN (
set EDT=ISO.BIN.EDAT
set ct=minis
set n1=1MNCAT
set title=PSPMINIS000000!ch!
)
if !cat!==PE (
set EDT=ISO.BIN.EDAT
set ct=Game_Data
set n1=1PECAT
set title=PSPREMASTERS00!ch!
)
if !cat!==PP (
set EDT=ISO.BIN.EDAT
set ct=Game_Data
set n1=1PPCAT
set title=PSPGAME0000000!ch!
)
echo.
echo Making DEBUG PKG...|!col! 0A
"!tmp!\sfoprint" "!tmp!\PARAM.SFO" TITLE|!col! 0E
if defined cid echo ContentID: !cid!|!col! 0D
"!tmp!\sfoprint" "!tmp!\PARAM.SFO" TITLE_ID|!col! 0B
echo GAME_DIR : !dnm! : !fn!|!col! 09
if defined cid set title=!cidend!!ch!
:: ??????? ??????
echo Content-ID = !n1!-!dnm:~0,9!_00-!title! > !conf!
echo k_licensee = 0x00000000000000000000000000000000 >> !conf!
echo DRM_Type = !DRM! >> !conf!
echo Content_Type = !ct! >> !conf!
if defined pt echo PackageType = !pt! >> !conf!
echo InstallDirectory = !dnm! >> !conf!
echo PackageVersion = !apver! >> !conf!
:: ???????????? Debug PKG
"!tmp!\psn_package_npdrm" -n -f -o "!src!" !conf! "!fnm!\SPLITTED\!fn!\!dnm!"
echo Move !fn! files from the SPLITTED folder back to the original folder. Please wait...|!col! 02
for /f "usebackq tokens=* delims=" %%B in ("%%A") do (
set pn=%%B
move "!fnm!\SPLITTED\!fn!\%%~B" "!fnm!\!pn:%%~nxB=!" > nul
)
del /q "!fnm!\!fn!.txt" && rd /s /q "!fnm!\SPLITTED"
echo.
if exist "!src!\!n1!-!dnm:~0,9!_00-!title!.pkg" (
set /a deb+=1
title !deb! Debug PKG created
echo DONE^^! !deb! Debug PKG created: "!n1!-!dnm:~0,9!_00-!title!.pkg"|!col! 0D
set debug=!deb! Debug PKG: !n1!-!dnm:~0,9!_00-!title!.pkg
echo.
if !EDT!==ISO.BIN.EDAT (
echo Signing DEBUG PKG to RETAIL...|!col! 0A
"!tmp!\sfoprint" "!tmp!\PARAM.SFO" TITLE|!col! 0E
if defined cid echo ContentID: !cid!|!col! 0D
"!tmp!\sfoprint" "!tmp!\PARAM.SFO" TITLE_ID|!col! 0B
echo.
echo | "!tmp!\ps3xploit_rifgen_edatresign" "!src!\!n1!-!dnm:~0,9!_00-!title!.pkg"
echo.
del "!src!\!n1!-!dnm:~0,9!_00-!title!.pkg" /q
:: ???? ???? ??????? PKG ?? ?????????? ??????????, ????????? ? ????? BACKUP
if exist "!src!\!n1!-!dnm:~0,9!_00-!title!_signed.pkg" (
echo Found file from previous compilation: "!n1!-!dnm:~0,9!_00-!title!_signed.pkg"|!col! 06
echo The old file will be moved to the "BACKUP" folder.|!col! 06
if not exist "!src!\BACKUP" md "!src!\BACKUP"
move /y "!src!\!n1!-!dnm:~0,9!_00-!title!_signed.pkg" "!src!\BACKUP"
)
ren "!src!\!n1!-!dnm:~0,9!_00-!title!.pkg_signed.pkg" "!n1!-!dnm:~0,9!_00-!title!_signed.pkg"
echo.
set /a sig+=1
title !sig! Signed Retail PKG created
echo DONE^^! !sig! Signed Retail PKG created: "!n1!-!dnm:~0,9!_00-!title!_signed.pkg"|!col! 0D
set signed=!sig! Signed PKG: !n1!-!dnm:~0,9!_00-!title!_signed.pkg
echo.
)
)
)

:exit
