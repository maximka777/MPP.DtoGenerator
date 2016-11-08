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
    public class JsonClassInfoReader
    {
        public static List<ClassInfo> ReadClassInfoFromFile(string filename)
        {
            List<ClassInfo> result = new List<ClassInfo>();
            using (StreamReader streamReader = new StreamReader(filename))
            {
                using (JsonReader reader = new JsonTextReader(streamReader))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    result = serializer.Deserialize<List<ClassInfo>>(reader);
                }
            }
            return result;
        }
    }
}
