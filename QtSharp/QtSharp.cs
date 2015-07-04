﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using CppAbi = CppSharp.Parser.AST.CppAbi;

namespace QtSharp
{
	public class QtSharp : ILibrary
	{
	    private readonly string qmake;
	    private readonly string make;
	    private readonly string includePath;
	    private readonly string module;
	    private readonly string libraryPath;
	    private readonly string library;
	    private readonly IEnumerable<string> systemIncludeDirs;
	    private readonly string target;
	    private readonly string docs;

	    public QtSharp(string qmake, string make, string includePath, string libraryPath, string library, string target, IEnumerable<string> systemIncludeDirs, string docs)
	    {
	        this.qmake = qmake;
	        this.includePath = includePath.Replace('/', Path.DirectorySeparatorChar);
	        this.module = Regex.Match(library, @"Qt\d?(?<module>\w+)\.\w+$").Groups["module"].Value;
	        this.libraryPath = libraryPath.Replace('/', Path.DirectorySeparatorChar);
	        this.library = library;
	        this.target = target;
	        this.systemIncludeDirs = systemIncludeDirs;
	        this.make = make;
            this.docs = docs;
	    }

	    public void Preprocess(Driver driver, ASTContext lib)
	    {
            string qtModule = "Qt" + this.module;
	        string moduleIncludes = Path.Combine(this.includePath, qtModule);
	        foreach (TranslationUnit unit in lib.TranslationUnits.Where(u => u.FilePath != "<invalid>"))
	        {
	            if (Path.GetDirectoryName(unit.FilePath) != moduleIncludes)
	            {
                    LinkDeclaration(unit);
	            }
	            else
	            {
	                IgnorePrivateDeclarations(unit);
	            }
	        }
            lib.SetClassAsValueType("QByteArray");
            lib.SetClassAsValueType("QListData");
            lib.SetClassAsValueType("QListData::Data");
            lib.SetClassAsValueType("QLocale");
            lib.SetClassAsValueType("QModelIndex");
            lib.SetClassAsValueType("QPoint");
            lib.SetClassAsValueType("QPointF");
            lib.SetClassAsValueType("QSize");
            lib.SetClassAsValueType("QSizeF");
            lib.SetClassAsValueType("QRect");
            lib.SetClassAsValueType("QRectF");
            lib.SetClassAsValueType("QGenericArgument");
            lib.SetClassAsValueType("QGenericReturnArgument");
            lib.SetClassAsValueType("QVariant");
            lib.IgnoreClassMethodWithName("QString", "fromStdWString");
            lib.IgnoreClassMethodWithName("QString", "toStdWString");
		}

	    private static void LinkDeclaration(Declaration declaration)
	    {
	        declaration.GenerationKind = GenerationKind.Link;
            DeclarationContext declarationContext = declaration as DeclarationContext;
            if (declarationContext != null)
            {
                foreach (var nestedDeclaration in declarationContext.Declarations)
                {
                    LinkDeclaration(nestedDeclaration);
                }
            }
	    }

	    private static void IgnorePrivateDeclarations(DeclarationContext unit)
	    {
	        foreach (var declaration in unit.Declarations)
	        {
	            IgnorePrivateDeclaration(declaration);
	        }
	    }

	    private static void IgnorePrivateDeclaration(Declaration declaration)
	    {
	        if (declaration.Name != null &&
	            (declaration.Name.StartsWith("Private") || declaration.Name.EndsWith("Private")))
	        {
	            declaration.ExplicityIgnored = true;
	        }
	        else
	        {
	            DeclarationContext declarationContext = declaration as DeclarationContext;
	            if (declarationContext != null)
	            {
	                IgnorePrivateDeclarations(declarationContext);
	            }
	        }
	    }

        public void Postprocess(Driver driver, ASTContext lib)
        {
            new ClearCommentsPass().VisitLibrary(driver.ASTContext);
            new GetCommentsFromQtDocsPass(this.docs, this.module).VisitLibrary(driver.ASTContext);
            new CaseRenamePass(
                RenameTargets.Function | RenameTargets.Method | RenameTargets.Property | RenameTargets.Delegate | RenameTargets.Field | RenameTargets.Variable,
                RenameCasePattern.UpperCamelCase).VisitLibrary(driver.ASTContext);
        }

		public void Setup(Driver driver)
		{
			driver.Options.GeneratorKind = GeneratorKind.CSharp;
		    string qtModule = "Qt" + this.module;
            driver.Options.addDefines("_CRTIMP=");
		    driver.Options.MicrosoftMode = false;
            driver.Options.TargetTriple = this.target;
            driver.Options.Abi = CppAbi.Itanium;
		    driver.Options.LibraryName = string.Format("{0}Sharp", qtModule);
		    driver.Options.OutputNamespace = qtModule;
			driver.Options.Verbose = true;
		    driver.Options.GenerateInterfacesForMultipleInheritance = true;
            driver.Options.GeneratePropertiesAdvanced = true;
			driver.Options.IgnoreParseWarnings = true;
		    driver.Options.CheckSymbols = true;
            driver.Options.GenerateSingleCSharpFile = true;
		    driver.Options.GenerateInlines = true;
		    driver.Options.CompileCode = true;
		    driver.Options.GenerateCopyConstructors = true;
		    driver.Options.GenerateDefaultValuesForArguments = true;
		    driver.Options.MarshalCharAsManagedChar = true;
            driver.Options.Headers.Add(qtModule);
		    foreach (var systemIncludeDir in this.systemIncludeDirs)
		    {
		        driver.Options.addSystemIncludeDirs(systemIncludeDir);
		    }
            driver.Options.addIncludeDirs(this.includePath);
            driver.Options.addIncludeDirs(Path.Combine(this.includePath, qtModule));
            driver.Options.addLibraryDirs(this.libraryPath);
            driver.Options.Libraries.Add(this.library);
		    if (this.module == "Core")
		    {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                driver.Options.CodeFiles.Add(Path.Combine(dir, "QEventArgs.cs"));
                driver.Options.CodeFiles.Add(Path.Combine(dir, "QEventHandler.cs"));
                driver.Options.CodeFiles.Add(Path.Combine(dir, "QObject.cs"));
                driver.Options.CodeFiles.Add(Path.Combine(dir, "QChar.cs"));
                driver.Options.CodeFiles.Add(Path.Combine(dir, "_iobuf.cs"));
		    }
		}

		public void SetupPasses(Driver driver)
		{
            driver.TranslationUnitPasses.AddPass(new CompileInlinesPass(this.qmake, this.make));
            driver.TranslationUnitPasses.AddPass(new GenerateEventEventsPass());
            driver.TranslationUnitPasses.AddPass(new GenerateSignalEventsPass());
		}
	}
}
