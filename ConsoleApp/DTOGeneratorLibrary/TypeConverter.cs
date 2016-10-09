using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOGeneratorLibrary
{
    class TypeConverter : ITypeConverter
    {
        private static readonly TypeConverter converter = new TypeConverter();

        Dictionary<ClassInfo, string> typeDictionary;

        private TypeConverter()
        {
            typeDictionary = new Dictionary<ClassInfo, string>();
        }

        public string tryGetTypeName(TypeInfo typeInfo)
        {
            return "System.Integer";
        }
    }
}
