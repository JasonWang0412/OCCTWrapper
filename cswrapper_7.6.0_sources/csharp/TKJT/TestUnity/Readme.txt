// Created: 2019-06-05
//
// Copyright (c) 2019-2021 OPEN CASCADE SAS
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

Compile and install project;
In installation directory edit StartUnity.bat and set path to unity.exe;
Run unity using StartUnity.bat to set environment variables necessary for Jt dunctionality;
Create new project with template 3D, e.g. D:/TestJt;
Copy content of Unity folder from installation directory to Assets directory of TestJt project, i.e. D:/TestJt/Assets;
Go to Unity and click camera object in the Scene view;
In Inspector window click Add Component and select TestJt;
Click play button and check console output for output of the TestJt script;
File cam.obj should appear in the Assets directory as a result of conversion of test cam.jt model.
