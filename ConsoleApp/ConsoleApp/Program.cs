using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using DTOGeneratorLibrary;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ClassInfo> classInfoList = JsonClassInfoReader.ReadClassInfoFromFile("info.json");
            DTOGenerator generator = new DTOGenerator();
            Dictionary<string, CompilationUnitSyntax> dict = generator.GenerateAllDTO(classInfoList);
            SyntaxFileWriter writer = new SyntaxFileWriter("generated_classes");
            writer.WriteAllSyntax(dict);
            Console.Read();
        }
    }
}
