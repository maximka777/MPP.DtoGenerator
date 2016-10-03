using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DTOGeneratorLibrary
{
    public class DTOGenerator
    {
        public void GenerateDTOClasses(List<ClassInfo> classInfoList)
        {

        }

        private string GenerateProperty(PropertyInfo propertyInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("public {0} {1} {{ get; set; }}", propertyInfo.Type, propertyInfo.Name);
            return stringBuilder.ToString();
        }

        public string GenerateDTO(ClassInfo classInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("public sealed class {0} {{\n", classInfo.ClassName);
            foreach(PropertyInfo propertyInfo in classInfo.properties){
                stringBuilder.AppendFormat("\t{0}\n", GenerateProperty(propertyInfo));
            }
            stringBuilder.Append("}}");
            return stringBuilder.ToString();
        }
    }
}
