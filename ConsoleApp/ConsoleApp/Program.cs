using System;
using System.Collections.Generic;
using DTOGeneratorLibrary;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Concurrent;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);
            string pathToDir = args[2];
            string pathToFile = args[0];
            int maxThreadCount = Int32.Parse(args[1]);
            List<ClassInfo> classInfoList = JsonClassInfoReader.ReadClassInfoFromFile(pathToFile);
            DTOGenerator generator = new DTOGenerator(maxThreadCount);
            Dictionary<string, CompilationUnitSyntax> dict = generator.GenerateAllDTO(classInfoList);
            SyntaxFileWriter writer = new SyntaxFileWriter(pathToDir);
            writer.WriteAllSyntax(dict);
        }
    }
}
