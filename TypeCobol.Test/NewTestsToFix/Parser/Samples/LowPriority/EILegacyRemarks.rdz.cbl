﻿000000*Don't except any errors
000010 IDENTIFICATION DIVISION.
000020 PROGRAM-ID. MYPGM.
000030*Remarks directive is not our standard but it works
000420*REMARKS. COPY=YXXX001
000430*         COPY=YXXX002
000440*         COPY=YXXX003
000450
000460**********************
000470 ENVIRONMENT DIVISION.
000480**********************
000490 CONFIGURATION SECTION.
000500 SOURCE-COMPUTER. IBM-3090.
000520 SPECIAL-NAMES. DECIMAL-POINT IS COMMA.
000900********************************
000910 PROCEDURE DIVISION.
001520     goback.