# Qt 5.8.0 Patches

At the time of writing, there are a couple of bugs associated with the latest Qt 5.8.0, which affect QtSharp. <br>
This just requires a couple of changes to the installed source files. <br>
with any luck these will be fixed within future versions of Qt.

There are a couple of quick changes we need to make for this version of Qt after it's been installed in order for this to work with QtSharp. <br>
The below assumes Qt has been installed to a path of C:\Qt\Qt5.8.0\

## Fix 1

  * <https://bugreports.qt.io/browse/QTBUG-55951>

Edit the file **C:\Qt\Qt5.8.0\5.8\mingw53_32\include\QtGui\qopenglversionfunctions.h**

On line 130 we need to add some brackets after the **init** statement

```CPP
    void init() {}
```


## Fix 2

  * <https://bugreports.qt.io/browse/QTBUG-58432>

For this one we just need to copy and paste the contents from one file to another

Copy the contents from <br>
**C:\Qt\Qt5.8.0\5.8\mingw53_32\include\QtMultimedia\qtmultimediadefs.h** <br>
To <br>
**C:\Qt\Qt5.8.0\5.8\Src\qtmultimedia\src\multimedia\qtmultimediadefs.h**
