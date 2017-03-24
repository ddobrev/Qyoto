# CppSharp Notes

## Overview

At the time of writing there has been a few changes to the API of CppSharp which may influence building QtSharp against it

  * module.TemplatesLibraryName seems to no longer exist
  * module.InlinesLibraryName I think has been renamed to module.SymbolsLibraryName
  * GenerateInlinesPass I think has been renamed to GenerateSymbolsPass
  * Basically the word Inlines seems to have been substituted with Symbols
  * For the CppSharp.Parser.CSharp reference I had to set the Aliases field to something other than global <br>
    to avoid a namespace conflict with CppAbi
