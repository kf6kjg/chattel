#!/bin/bash

cd bin

BABOON_CFG="Chattel.covcfg" mono ../../XR.Baboon/covtool/bin/covem.exe ../../nunit-console/bin/Debug/nunit3-console.exe ChattelTests.dll Chattel-AssetToolsTests.dll

mono ../../XR.Baboon/covtool/bin/cov-srchtml.exe ../Chattel.covcfg.covdb ../Source ../covem.report