# QtSharp Build - Windows

## Overview

When building QtSharp the general order of things is

* Build QtSharp
* Run QtSharp.CLI (Generate Bindings Code)

Optionally:

* Run NUnit Tests
* Build CppSharp
* Rebuild QtSharp using a newer built version of CppSharp

For this document, we will assume you are using the default version of CppSharp, which is pulled via NuGet during the build. <br>
For using a more recent version of CppSharp see the section on building CppSharp.

## Setting the Windows Paths

One approach I have found for getting things to work with Qt and QtSharp under Windows <br>
Is to make sure that the windows paths include a couple of Qt directories

  * Open up the **Control Panel**
  * Select **System**
  * Select **Advanced System Settings** on the left hand side
  * Select the **Advanced** Tab
  * Click **Environmental Variables**
  * Under **System variables** Select the **Path** entry and click **Edit**

<a href="../../images/Win-Build/QtSharp-Build-Win/Qt-Build-Path1.png"><img src="../../images/Win-Build/QtSharp-Build-Win/Qt-Build-Path1.png" height="50%" width="50%" ></a> <br><br>

  * Next, make sure the following directory entries are located in the Path <br>
    These may need to be adjusted based on the version of Qt

<a href="../../images/Win-Build/QtSharp-Build-Win/Qt-Build-Path2.png"><img src="../../images/Win-Build/QtSharp-Build-Win/Qt-Build-Path2.png" height="50%" width="50%" ></a> <br><br>


## Downloading the Sources

### Using git to clone the sources

First, we need to clone the git repository. <br>
Let us assume you are using a directory of C:\GITHUB\ <br>
In addition, you will need to be using a Visual Studio 2015 console for the correct exe's to be found.

```bash
mkdir C:\GITHUB
cd C:\GITHUB\
git clone https://github.com/ddobrev/QtSharp.git QtSharp
cd C:\GITHUB\QtSharp
git checkout master
```

### Creating the Project / Solution files with premake

TODO

### Downloading NuGet packages

Next we need to tell NuGet to download any missing depends into a packages sub-directory <br>


To do this at the command line

```bash
nuget.exe restore QtSharp.sln
```

To do this within Visual Studio

  * Open the solution within Visual Studio
  * Right click the solution
  * Select **Restore NuGet Packages**

This should create a packages sub-directory and download any needed external libraries. <br>
Note attempting to build within visual studio should also trigger this automatically.


## Building QtSharp / QtSharp.CLI

Next, we are going to build the apps and libraries needed to generate the code


### Visual Studio 2015

To build the sources under Visual Studio 2015

  * Open up the QtSharp.sln File within Visual Studio
  * Right Click **Build** on the QtSharp / QtSharp.CLI Projects


### Windows Command Line

In order to build QtSharp.CLI from the command line

```bash
msbuild QtSharp.sln /p:Configuration=Debug;Platform="Any CPU" /m
```

## Running QtSharp.CLI

Next we need to run QtSharp.CLI to generate the bindings / C# Libraries needed against Qt


### Running from the command line

To run QtSharp from the command line

```bash
cd C:\GITHUB\QtSharp\QtSharp.CLI\bin\Debug
QtSharp.CLI.exe C:\Qt\Qt5.8.0\5.8\mingw53_32\bin\qmake.exe C:\Qt\Qt5.8.0\Tools\mingw530_32\bin\mingw32-make.exe
```

Note the paths will likely be different based on the version of QtSharp you have installed.
Also, watch out for the naming of the mingw directories, as these tend to change between Qt releases as well.


### Running from Visual Studio

To run the build from within visual studio we just need to set the Debug settings within project properties

<a href="../../images/Win-Build/QtSharp-Build-Win/Qt-Build-1.png"><img src="../../images/Win-Build/QtSharp-Build-Win/Qt-Build-1.png" height="50%" width="50%" ></a> <br><br>


## Build Result

The build result should end up within a zip file in the same directory as the built exe <br>
**QtSharp.zip**

<a href="../../images/Win-Build/QtSharp-Build-Win/Qt-Build-2.png"><img src="../../images/Win-Build/QtSharp-Build-Win/Qt-Build-2.png" height="50%" width="50%" ></a> <br><br>
