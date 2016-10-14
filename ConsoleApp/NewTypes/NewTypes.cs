using DTOGeneratorLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTypes
{
    public class NewTypes : ITypeConverter
    {
        public string TryGetTypeName(TypeInfo typeInfo)
        {
            return "System.Char";
        }
    }
}
