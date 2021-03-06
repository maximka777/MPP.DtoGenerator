﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    class SyntaxFileWriter
    {
        private string fileExtension = ".cs";
        private string directoryName;

        public SyntaxFileWriter(string directoryName)
        {
            this.directoryName = directoryName;
        }

        public void WriteAllSyntax(ConcurrentDictionary<string, CompilationUnitSyntax> syntaxDictionary)
        {
            foreach(string key in syntaxDictionary.Keys)
            {
                WriteSyntax(key, syntaxDictionary[key]);
            }
        }

        public string GenerateCodeString(CompilationUnitSyntax syntax)
        {
            SyntaxNode formattedNode = Formatter.Format(syntax, MSBuildWorkspace.Create());
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                formattedNode.WriteTo(writer);
            }
            return sb.ToString();
        }

        private string GeneratePathToFile(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), directoryName, name + fileExtension);
        }

        public void WriteSyntax(string name, CompilationUnitSyntax syntax)
        {
            using (TextWriter writer = File.CreateText(GeneratePathToFile(name)))
            {
                writer.Write(GenerateCodeString(syntax));
            }
        }
    
    }
}
