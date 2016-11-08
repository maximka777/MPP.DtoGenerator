using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MultiThreading;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace DTOGeneratorLibrary
{
    public class DTOGenerator
    {
        private int maxThreadCount;
        private TypeService typeService;

        public DTOGenerator(int maxThreadCount = 10)
        {
            this.maxThreadCount = maxThreadCount;
            this.typeService = TypeService.Instance;
        }

        public Dictionary<string, CompilationUnitSyntax> GenerateAllDTO(List<ClassInfo> classInfoList)
        { 
            ThreadPool<ClassInfo, CompilationUnitSyntax> threadPool = 
                new ThreadPool<ClassInfo, CompilationUnitSyntax>(maxThreadCount, GenerateDTO);
            foreach (ClassInfo classInfo in classInfoList)
            {
                threadPool.AddTask(classInfo);
            }
            threadPool.Wait();
            ConcurrentBag<CompilationUnitSyntax> result = threadPool.ResultList;
            Dictionary<string, CompilationUnitSyntax> resultDict = new Dictionary<string, CompilationUnitSyntax>();
            foreach(CompilationUnitSyntax syntax in result)
            {
                resultDict.Add(GetNameFromCompilationUnitSyntax(syntax), syntax);
            }
            return resultDict;
        }

        private string GetNameFromCompilationUnitSyntax(CompilationUnitSyntax syntax)
        {
            return ((ClassDeclarationSyntax)syntax.Members[0]).Identifier.ToString();
        }

        private TypeSyntax GenerateType(string type, string format)
        {
            string _NetTypeName = typeService.GetType(new TypeInfo(type, format));
            return SyntaxFactory.ParseTypeName(_NetTypeName);
        }

        private PropertyDeclarationSyntax GenerateProperty(PropertyInfo propertyInfo)
        {
            TypeSyntax type = GenerateType(propertyInfo.Type, propertyInfo.Format);
            PropertyDeclarationSyntax property = SyntaxFactory.PropertyDeclaration(type, propertyInfo.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).
                    WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).
                    WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            return property;
        }

        private PropertyDeclarationSyntax[] GenerateAllProperties(List<PropertyInfo> propertyInfoList)
        {
            PropertyDeclarationSyntax[] properties = new PropertyDeclarationSyntax[propertyInfoList.Count];
            int i = 0;
            foreach(PropertyInfo propertyInfo in propertyInfoList)
            {
                properties[i++] = GenerateProperty(propertyInfo);
            }
            return properties;
        }

        private ClassDeclarationSyntax GenerateClass(ClassInfo classInfo)
        {
            ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration(SyntaxFactory.Identifier(classInfo.ClassName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddMembers(GenerateAllProperties(classInfo.Properties));
            return classDeclarationSyntax;
        }

        public CompilationUnitSyntax GenerateDTO(ClassInfo classInfo)
        {
            CompilationUnitSyntax compilationUnitSyntax = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")))
                .AddMembers(GenerateClass(classInfo));
            return compilationUnitSyntax;
        }
    }
}
