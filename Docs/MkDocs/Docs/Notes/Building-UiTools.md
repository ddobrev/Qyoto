# Building UITools

## Overview

I've left this doc in, but with the more recent version of Qt I've had trouble getting UITools to build.
So it may be worth just relying on the QtDesigner wrapper instead.

One of the features that Qt has is the ability to create .ui files via Qt Designer.
These can be thought of as similar to windows form objects within Visual Studio.
UI files are basically just an xml file describing the layout of buttons and controls on a Qt Widget or Main Window

Normally within C++ it's possible to compile these .ui files into C++, or dynamically load them at runtime from a text file or resource.
With .Net / QtSharp it should be easier to load these dynamically at runtime instead of trying to compile them into C++.


## Methods for loading UI Files

### QTDesigner

There are two approaches to loading in a ui file at runtime, the first approach is to use the QtDesigner module (Qt5Designer.dll).
There is a class called QFormBuilder with a load method to load in .ui files

  * <http://doc.qt.io/qt-5/QFormBuilder.html>

### UITools

The second approach is to try and use UITools / QUiLoader.
The UItools module appears to be a strimmed down version of QTDesigner, meant for use with runtime only for just loading ui files.
The library is only currently built statically via Qt, not dynamically as a dll which prevents CppSharp from accessing it currently.

  * <http://doc.qt.io/qt-5/quiloader.html>


## Building Qt5UiTools.dll under windows

If you want to experiment building this as a dll under Windows:

 * First install Qt5.8.0 (or whichever the latest version is)
 * During the install make sure to select the mingw tools, and to install the source code
 * make sure qmake is located within your path

```bash
#Next we want to change directory to the src\uitools subdirectory
cd C:\Qt\Qt5.8.0\5.8\Src\qttools\src\designer\src\uitools

#Next run qmake to generate the makefiles
qmake

# Run make to build out the dll
C:\Qt\Qt5.8.0\Tools\mingw530_32\bin\mingw32-make.exe clean
C:\Qt\Qt5.8.0\Tools\mingw530_32\bin\mingw32-make.exe
```

 * At this stage the dll's **Qt5UiTools.dll**, **Qt5UiToolsd.dll** should show up under the directory C:\Qt\Qt5.5.0\5.5\Src\qttools\lib\
 * Copy these into the main bin directory along side the other dll's C:\Qt\Qt5.5.0\5.5\mingw492_32\bin\
