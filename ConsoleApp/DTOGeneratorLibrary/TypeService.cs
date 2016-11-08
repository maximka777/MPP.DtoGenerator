using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DTOGeneratorLibrary
{
    private class TypeService
    {
        private static readonly TypeService service = new TypeService();
        private static string rootDirectoryName;
        private List<ITypeConverter> foreignTypeConverters = new List<ITypeConverter>();

        private TypeService()
        {
            rootDirectoryName = Path.Combine(Directory.GetCurrentDirectory(), "mods", "types");
            LoadForeignTypeConverters();
        }

        public static TypeService Instance {
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
            return String.Empty;
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
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(filename);
            Assembly assembly = Assembly.Load(assemblyName);
            foreach(System.Reflection.TypeInfo type in assembly.GetTypes())
            {
                if (type.GetInterface(typeof(ITypeConverter).FullName) != null)
                {
                    yield return (ITypeConverter)Activator.CreateInstance(type);
                }
            }
            yield break;
        }


        private bool IsImplementingInterface(Type type, Type interfaceType)
        {
            Console.WriteLine(type);
            foreach(Type implementedInterface in type.GetInterfaces())
            {
                if(implementedInterface == interfaceType)
                {
                    return true;
                }
            }
            return false;
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
                try {
                    result = converter.TryGetTypeName(typeInfo);
                }
                catch(Exception exc)
                {
                    result = null;
                }
                if(result != null)
                {
                    return result;
                }
            }
            return null;
        } 
    }
}
