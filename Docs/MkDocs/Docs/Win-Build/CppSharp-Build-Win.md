# CppSharp Build - Windows

## Overview

One of the main dependencies for QtSharp is CppSharp.
The default version of CppSharp used for QtSharp is pulled from NuGet.
However currently CppSharp and QtSharp are still under development so using a more recent version of CppSharp is useful for development purposes.
CppSharp is where most of the work is done for generating the bindings.

Currently at the time of writing, the API associated with Cppsharp might be out of sync with the one used with QtSharp.
Be aware that you may need to modify QtSharp to get it to work with the latest version of CppSharp.
I believe one of the more recent changes is the rename of "inlines" to "symbols" within the API for example.

  * It is probably a good idea to make sure to build it within directories that do not have spaces in the path name<br>
    (at least for a windows box anyway)
  * [CppSharp - Getting Started](https://github.com/mono/CppSharp/blob/master/docs/GettingStarted.md)
  * [CppSharp Notes](../Notes/CppSharp-Notes.md)

Also for this build, you will need git and cmake installed for windows.


## Downloading Sources

### Cloning CppSharp

First, we need to clone the CppSharp sources <br>
For this example, I am using version 1de852ad9b0fd52ba120df3922e93463f2e08696 <br>
However, the latest release is probably the best for experimentation <br>

```bash
mkdir C:\GITHUB
cd C:\GITHUB\
git clone https://github.com/mono/CppSharp CppSharp
cd C:\GITHUB\cppsharp
git checkout master
```

If you have already cloned the sources and just want to update:
```bash
cd C:\GITHUB\cppsharp
git pull
git checkout master
```

To see which version the sources are at:
```bash
git rev-parse HEAD
```

To switch across to a specific version
```bash
git checkout 1de852ad9b0fd52ba120df3922e93463f2e08696
```

There is a few different forks available

* [CppSharp - Original Upstream](https://github.com/mono/CppSharp.git)
* [CppSharp - GoldRanks fork](https://github.com/golddranks/CppSharp.git)
* [CppSharp - ddobrev's tree](https://github.com/ddobrev/CppSharp.git)


### Cloning LLVM

One of the dependencies for CppSharp is LLVM. <br>
Within the CppSharp docs they recommend using a pre-built version.
However, I have discovered at the time of writing that this seems to be compiled for 32bit instead of 64bit.
By default, QtSharp is 64bit, so we are going to build CppSharp the same way.

The recommended commit id is [located here](https://github.com/mono/CppSharp/blob/master/build/LLVM-commit)

```bash
cd C:\GITHUB\CppSharp\deps\
git clone http://llvm.org/git/llvm.git llvm
cd C:\GITHUB\CppSharp\deps\llvm
git checkout cccdd2eff6e04737dfc4a2caf33ea1a6ee5fb80e
```

### Cloning Clang

Next, we need to clone Clang <br>
The recommended commit id is [located here](https://github.com/mono/CppSharp/blob/master/build/Clang-commit)

``` bash
cd C:\GITHUB\CppSharp\deps\llvm\tools\
git clone http://llvm.org/git/clang.git clang
cd C:\GITHUB\CppSharp\deps\llvm\tools\clang
git checkout 6a795889c33eb039866a867b41c30fdf000e345d
```


## Building LLVM / Clang

Next, we are going to build LLVM and Clang. which are depends of CppSharp. Clang will be built as part of this process automatically <br>
To get things to match up we're going to use a 64 bit build. <br>
Note that while QtSharp and QtSharp.CLI run as x64 by default, it is possible just to change the arch to x32 in the project settings <br>
In addition, the outputted Dll's built for the wrapper for Qt will actually be x32 by default.

First, let's create a build directory within the llvm directory

```bash
cd C:\GITHUB\CppSharp\deps\llvm
mkdir build
cd build
```

Next, let's run cmake to generate the project files and msbuild to build LLVM / Clang. <br>
Make sure to run this within a **Visual Studio 2015 Command prompt** so that it can find the msbuild command <br>
This selects **RelWithDebInfo** as the build configuration <br>
Version 14 of visual studio corresponds to Visual Studio 2015; this value needs to match the version of Visual Studio used for CppSharp.

### For 64Bit

```bash
cmake -G "Visual Studio 14 Win64" -DCLANG_BUILD_EXAMPLES=false -DCLANG_INCLUDE_DOCS=false -DCLANG_INCLUDE_TESTS=false -DCLANG_INCLUDE_DOCS=false -DCLANG_BUILD_EXAMPLES=false -DLLVM_TARGETS_TO_BUILD="X86" -DLLVM_INCLUDE_EXAMPLES=false -DLLVM_INCLUDE_DOCS=false -DLLVM_INCLUDE_TESTS=false ..
msbuild LLVM.sln /p:Configuration=RelWithDebInfo;Platform=x64 /m
```

### For 32bit:

```bash
cmake -G "Visual Studio 14" -DCLANG_BUILD_EXAMPLES=false -DCLANG_INCLUDE_DOCS=false -DCLANG_INCLUDE_TESTS=false -DCLANG_INCLUDE_DOCS=false -DCLANG_BUILD_EXAMPLES=false -DLLVM_TARGETS_TO_BUILD="X86" -DLLVM_INCLUDE_EXAMPLES=false -DLLVM_INCLUDE_DOCS=false -DLLVM_INCLUDE_TESTS=false ..
msbuild LLVM.sln /p:Configuration=RelWithDebInfo;Platform=Win32 /m
```


## Building CppSharp

### Generating the Project Files

To build CppSharp we first need to generate the Project Solution files using premake.

```
cd C:\GITHUB\CppSharp\build
GenerateProjects.bat
Choose option 3 / Visual Studio 2015
```

Note if you have used the script to download the build for llvm.
This typically shows up within a subdirectory in the build directory here.
The lua script called from GenerateProjects.bat will typically use this directory in favour of the deps/llvm one if it is discovered.

In addition, it looks as if CppSharp is now using C# v6 features; this means the version of Visual Studio needs to be 2015 or higher.


### Building under Visual Studio 2015

To build the sources for CppSharp under Visual Studio

  * Open up the **build\vs2015\CppSharp.sln** solution file
  * Select Release from the top menu of Visual Studio
  * Right click on the Libraries solution folder and select Build

<a href="../../images/Win-Build/CppSharp-Build-Win/CppSharp-Build-1.png"><img src="../../images/Win-Build/CppSharp-Build-Win/CppSharp-Build-1.png" height="50%" width="50%" ></a> <br><br>

I find its better just to build the Libraries folder instead of all the tests with the latest master version of CppSharp.


### Building via the Command Line

To build via the command line, make sure to open up a Visual Studio 2015 command prompt

For 64bit:
```
msbuild vs2013\CppSharp.sln /p:Configuration=Release;Platform=x64
```

For 32bit:
```
msbuild vs2013\CppSharp.sln /p:Configuration=Release;Platform=x86
```

## Copying the Built files

The dll output build should now be within Release_x64, or Release_x32 for 32bit

```
C:\GITHUB\CppSharp\build\vs2015\lib\Release_x64\
```

### Reference Files

The following files are referenced by the QtSharp project. <br>
Typically, you will see these within the QtSharp\packages\CppSharp.0.8.6\lib directory

```
CppSharp.AST.dll
CppSharp.dll
CppSharp.Generator.dll
CppSharp.Parser.CSharp.dll
CppSharp.Parser.dll
CppSharp.Runtime.dll
```

With the latest CppSharp it looks like the following is also required as an additional reference

```
CppSharp.Parser.CLI.dll
```


### C++ / Headers to Copy

The following files are typically dumped into the directory of the built QtSharp.CLI exe. <br>
These are not referenced by the QtSharp exe's / libs directly but they are used. <br>
Typically, they show up within QtSharp\packages\CppSharp.0.8.6\output\

Native C++ Library
```
CppSharp.CppParser.dll
```

Includes directory
```
CppSharp\deps\llvm\build\RelWithDebInfo\lib\clang\3.9.0\include
```

### Switching Versions

One quick way I have found to switch the version of CppSharp used within QtSharp quickly.

  * Backup the QtSharp\packages\CppSharp.0.8.6 directoty to QtSharp\packages\CppSharp.0.8.6.bak
  * overwrite the files within QtSharp\packages\CppSharp.0.8.6\lib with those mentioned above as References
  * overwrite the files within QtSharp\packages\CppSharp.0.8.6\output with those mentioned above as C++ / Headers to Copy
  * remove and re-add the references for CppSharp within the QtSharp libraries
