// Created: 2016-07-05
//
// Copyright (c) 2016-2021 OPEN CASCADE SAS
//
// This file is part of commercial software by OPEN CASCADE SAS.
//
// This software is furnished in accordance with the terms and conditions
// of the contract and with the inclusion of this copyright notice.
// This software or any other copy thereof may not be provided or otherwise
// be made available to any third party.
//
// No ownership title to the software is transferred hereby.
//
// OPEN CASCADE SAS makes no representation or warranties with respect to the
// performance of this software, and specifically disclaims any responsibility
// for any damages, special or consequential, connected with its use.

// Define list of necessary libraries to link with
%{
#ifdef _MSC_VER
#pragma comment(lib, "TKOCCLicense.lib")
#endif
%}

%{
#include <OCCLicense_Activate.hxx>
%}

extern void OCCLicense_Activate (const char *product, const char *key);

extern bool OCCLicense_LoadKeyFile (const char* thePath, const bool theIsVerbose);

extern unsigned long OCCLicense_GetHostID (const int theNetworkAdapterNum = 0);

extern void OCCLicense_GetLicenseInfo(Standard_Character theHostId[32], TCollection_AsciiString &theLicensesInfo);
