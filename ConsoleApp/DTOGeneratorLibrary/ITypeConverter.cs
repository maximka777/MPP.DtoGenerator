using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOGeneratorLibrary
{
    public interface ITypeConverter
    {
        string tryGetTypeName(TypeInfo typeInfo);
    }
}
