﻿using System.Diagnostics;
using Foundation.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Foundation.Generators;

public class SyntaxReceiver : ISyntaxContextReceiver
{
    public MethodDeclarationSyntax MigrationFunction { get; set; } = null;
    public List<ITypeSymbol> MigrationClasses { get; } = new();
    public List<ITypeSymbol> MigrationClasses2 { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        try
        {
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0)
            {
                // Get the symbol being declared by the class, and keep it if its annotated
                ITypeSymbol classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as ITypeSymbol;

                if (classSymbol.GetAttributes().Any(data => data.AttributeClass.Name == "MigrationFunction2Attribute"))
                {
                    this.MigrationClasses2.Add(classSymbol);
                }

                if (classSymbol.GetAttributes().Any(attr => attr.AttributeClass.Name == "MigrationAttribute"))
                {
                    MigrationClasses.Add(classSymbol);
                }
            }

            // any method with at least one attribute is a candidate of function generation
            if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax && methodDeclarationSyntax.AttributeLists.Count > 0)
            {
                // Get the symbol being declared by the method, and keep it if its annotated
                var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
                if (methodSymbol.GetAttributes().Any(attr => attr.AttributeClass.Name == nameof(MigrationFunctionAttribute)))
                {
                    MigrationFunction = methodDeclarationSyntax;
                }
            }

        }
        catch (Exception e)
        {

            throw;
        }

    }

}