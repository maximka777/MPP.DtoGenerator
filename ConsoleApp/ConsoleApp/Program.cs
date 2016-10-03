using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using DTOGeneratorLibrary;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ClassInfo> classInfoList = JsonClassInfoReader.ReadClassInfoFromFile("info.json");
            DTOGenerator generator = new DTOGenerator();
            generator.GenerateDTOClasses(classInfoList);
            Console.WriteLine(generator.GenerateDTO(classInfoList[0]));
            Console.Read();
        }
    }
}
