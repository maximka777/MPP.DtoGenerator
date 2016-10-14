using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DTOGeneratorLibrary
{
    class TypeService
    {
        private static readonly TypeService service = new TypeService();
        private static string rootDirectoryName;
        private List<ITypeConverter> foreignTypeConverters = new List<ITypeConverter>();

        private TypeService()
        {
            rootDirectoryName = Path.Combine(Directory.GetCurrentDirectory(), "mods", "types");
            LoadForeignTypeConverters();
        }

        public static TypeService Instsance {
            get
            {
                return service;
            }
        }

        public string GetType(TypeInfo typeInfo)
        {
            string result;
            result = TryGetFromStandardTypeConverter(typeInfo);
            if(result != null)
            {
                return result;
            }
            result = TryGetFromForeignTypeConverters(typeInfo);
            if (result != null)
            {
                return result;
            }
            return "";
        }
        
        private string TryGetFromStandardTypeConverter(TypeInfo typeInfo)
        {
            return TypeConverter.Instance.TryGetTypeName(typeInfo);
        }

        private void LoadForeignTypeConverters()
        {
            string[] dllNameList = Directory.GetFiles(rootDirectoryName, "*.dll");
            foreach (string dllName in dllNameList)
            {
                foreach(ITypeConverter converter in CheckFileOnImplementationOfITypeConverter(dllName))
                {
                    foreignTypeConverters.Add(converter);
                }
            }
        }

        private IEnumerable<ITypeConverter> CheckFileOnImplementationOfITypeConverter(string filename)
        {
            Assembly assembly = Assembly.LoadFrom(filename);
            foreach(System.Reflection.TypeInfo type in assembly.DefinedTypes)
            {
                if (type.FindInterfaces(ITypeConverterFilter, null).Length != 0)
                    yield return (ITypeConverter) Activator.CreateInstance(type);
            }
            yield break;
        }

        private static bool ITypeConverterFilter(Type type, object criteriaObject)
        {
            if (type is ITypeConverter)
                return true;
            return false;
        }

        private string TryGetFromForeignTypeConverters(TypeInfo typeInfo)
        {
            string result;
            foreach(ITypeConverter converter in foreignTypeConverters)
            {
                result = converter.TryGetTypeName(typeInfo);
                if(result != null)
                {
                    return result;
                }
            }
            return null;
        } 
    }
}
