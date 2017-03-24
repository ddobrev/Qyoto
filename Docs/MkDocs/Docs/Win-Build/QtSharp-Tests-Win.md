# NUnit Tests

## Overview

The tests use NUnit to run instead of the inbuilt Visual Studio test system (more cross platform) <br>


## Running NUnit Tests - Visual Studio

First, make sure the NUnit Test Adapter is installed for Visual Studio under the Extensions

  * Within Visual Studio 2015 Tools -> Extensions and Updates -> Install NUnit Test Adapter

Next we need to Build QtSharp.Tests (don't try to run it as a project, just build)

  * Right click the **QtSharp.Tests** project and select **Build**

Next make sure the test / platform setting is set to x32 to match the architecture setting of the generated QtSharp library

  * Test Menu -> Test Settings -> Default Processor Architecture -> X86

Next Run the Tests

  * Test Menu -> Windows -> Test Explorer
  * Wait a while for it to analyse the available tests
  * Click **Run All**
