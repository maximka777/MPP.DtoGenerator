using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DTOGeneratorLibrary
{
    public class DTOGenerator
    {
        public Dictionary<string, CompilationUnitSyntax> GenerateAllDTO(List<ClassInfo> classInfoList)
        {
            Dictionary<string, CompilationUnitSyntax> result = new Dictionary<string, CompilationUnitSyntax>();
            foreach (ClassInfo classInfo in classInfoList)
            {
                result.Add(classInfo.ClassName, GenerateDTO(classInfo));
            }
            return result;
        }

        private TypeSyntax GenerateType(string type, string format)
        {
            return SyntaxFactory.ParseTypeName("System.Integer");
        }

        private PropertyDeclarationSyntax GenerateProperty(PropertyInfo propertyInfo)
        {
            TypeSyntax type = GenerateType(propertyInfo.Type, propertyInfo.Format);
            PropertyDeclarationSyntax property = SyntaxFactory.PropertyDeclaration(type, propertyInfo.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
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
