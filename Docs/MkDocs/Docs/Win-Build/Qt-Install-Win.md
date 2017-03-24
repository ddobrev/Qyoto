# Qt Install - Windows

## Downloading Qt

This is a list of steps for installing the correct version of Qt for use with Windows and QtSharp. <br>
First, we are going to download Qt from the following link.

  * <https://www.qt.io/download/>

<br>

Select **Open source distribution under an LGPL or GPL license**

<a href="../../images/Win-Build/Qt-Install-Win/Qt-Install-1.png"><img src="../../images/Win-Build/Qt-Install-Win/Qt-Install-1.png" height="50%" width="50%" ></a> <br><br>


Select **Yes**

<a href="../../images/Win-Build/Qt-Install-Win/Qt-Install-2.png"><img src="../../images/Win-Build/Qt-Install-Win/Qt-Install-2.png" height="50%" width="50%" ></a> <br><br>


Select **Yes**

<a href="../../images/Win-Build/Qt-Install-Win/Qt-Install-3.png"><img src="../../images/Win-Build/Qt-Install-Win/Qt-Install-3.png" height="50%" width="50%" ></a> <br><br>


Select **Getting Started**

<a href="../../images/Win-Build/Qt-Install-Win/Qt-Install-4.png"><img src="../../images/Win-Build/Qt-Install-Win/Qt-Install-4.png" height="50%" width="50%" ></a> <br><br>


Select **View All Downloads**

<a href="../../images/Win-Build/Qt-Install-Win/Qt-Install-5.png"><img src="../../images/Win-Build/Qt-Install-Win/Qt-Install-5.png" height="30%" width="30%" ></a> <br><br>


For windows we want the **MinGW** version of QT under **Offline Installers**

<a href="../../images/Win-Build/Qt-Install-Win/Qt-Install-6.png"><img src="../../images/Win-Build/Qt-Install-Win/Qt-Install-6.png" height="30%" width="30%" ></a> <br><br>


## Installing Qt

Next, it is just a case of running the install wizard. <br>
Make sure to select the option for the source code and MinGW.

<a href="../../images/Win-Build/Qt-Install-Win/Qt-Install-7.png"><img src="../../images/Win-Build/Qt-Install-Win/Qt-Install-7.png" height="30%" width="30%" ></a> <br><br>


## Patching Qt

At the time of writing, there are a couple of bugs associated with the latest Qt 5.8.0, which affect QtSharp. <br>
This just requires a couple of changes to the installed source files. <br>
I have placed these within a separate document, with any luck these will be fixed within future versions of Qt.

  * [Qt 5.8.0 Notes](../Notes/Qt-5.8.0-Notes.md)
