﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using RoslynTool.CsToLua;

namespace RoslynTool.CsToLua
{
    internal class ClassSymbolInfo
    {
        internal string ClassKey = string.Empty;
        internal string BaseClassKey = string.Empty;
        internal bool ExistConstructor = false;
        internal bool ExistStaticConstructor = false;
        internal bool GenerateBasicCtor = false;
        internal bool GenerateBasicCctor = false;
        internal bool GenerateTypeParamFields = false;

        internal INamedTypeSymbol TypeSymbol = null;
        internal List<IFieldSymbol> FieldSymbols = new List<IFieldSymbol>();
        internal List<IMethodSymbol> MethodSymbols = new List<IMethodSymbol>();
        internal List<IPropertySymbol> PropertySymbols = new List<IPropertySymbol>();
        internal List<IEventSymbol> EventSymbols = new List<IEventSymbol>();
        internal Dictionary<string, bool> SymbolOverloadFlags = new Dictionary<string, bool>();
        internal Dictionary<string, IMethodSymbol> MethodIncludeTypeOfs = new Dictionary<string, IMethodSymbol>();
        internal Dictionary<string, IFieldSymbol> FieldIncludeTypeOfs = new Dictionary<string, IFieldSymbol>();
        internal List<string> GenericTypeParamNames = new List<string>();

        internal void Init(INamedTypeSymbol typeSym, CSharpCompilation compilation)
        {
            ClassKey = ClassInfo.GetFullName(typeSym);
            BaseClassKey = ClassInfo.GetFullName(typeSym.BaseType);
            if (BaseClassKey == "System.Object" || BaseClassKey == "System.ValueType")
                BaseClassKey = string.Empty;
            ExistConstructor = false;
            ExistStaticConstructor = false;

            INamedTypeSymbol type = typeSym;
            while (null != type) {
                if (type.IsGenericType) {
                    foreach (var param in type.TypeParameters) {
                        GenericTypeParamNames.Add(param.Name);
                    }
                }
                type = type.ContainingType;
            }

            bool fieldIncludeTypeOf = false;
            bool staticFieldIncludeTypeOf = false;
            TypeSymbol = typeSym;
            foreach (var sym in TypeSymbol.GetMembers()) {
                var fsym = sym as IFieldSymbol;
                if (null != fsym) {
                    FieldSymbols.Add(fsym);

                    if (typeSym.IsGenericType) {
                        CheckFieldIncludeTypeOf(fsym, compilation, ref fieldIncludeTypeOf, ref staticFieldIncludeTypeOf);
                    }
                    continue;
                }
            }
            foreach (var sym in TypeSymbol.GetMembers()) {
                var msym = sym as IMethodSymbol;
                if (null != msym) {
                    if (msym.MethodKind == MethodKind.Constructor && !msym.IsImplicitlyDeclared) {
                        ExistConstructor = true;
                    } else if (msym.MethodKind == MethodKind.StaticConstructor && !msym.IsImplicitlyDeclared) {
                        ExistStaticConstructor = true;
                    }
                    MethodSymbols.Add(msym);

                    string name = msym.Name;
                    if (name[0] == '.')
                        name = name.Substring(1);
                    if (!SymbolOverloadFlags.ContainsKey(name)) {
                        SymbolOverloadFlags.Add(name, false);
                    } else {
                        SymbolOverloadFlags[name] = true;
                    }

                    if (typeSym.IsGenericType) {
                        CheckMethodIncludeTypeOf(msym, compilation, false);

                        if (fieldIncludeTypeOf && msym.MethodKind == MethodKind.Constructor) {
                            string manglingName = SymbolTable.CalcMethodMangling(msym);
                            MethodIncludeTypeOfs.Add(manglingName, msym);
                        }
                        if (staticFieldIncludeTypeOf && msym.MethodKind == MethodKind.StaticConstructor) {
                            string manglingName = SymbolTable.CalcMethodMangling(msym);
                            MethodIncludeTypeOfs.Add(manglingName, msym);
                        }
                    }
                    continue;
                }
                var psym = sym as IPropertySymbol;
                if (null != psym) {
                    PropertySymbols.Add(psym);

                    if (typeSym.IsGenericType) {
                        if (null != psym.GetMethod) {
                            CheckMethodIncludeTypeOf(psym.GetMethod, compilation, true);
                        }
                        if (null != psym.SetMethod) {
                            CheckMethodIncludeTypeOf(psym.SetMethod, compilation, true);
                        }
                    }
                    continue;
                }
                var esym = sym as IEventSymbol;
                if (null != esym) {
                    EventSymbols.Add(esym);

                    if (typeSym.IsGenericType) {
                        if (null != esym.AddMethod) {
                            CheckMethodIncludeTypeOf(esym.AddMethod, compilation, true);
                        }
                        if (null != esym.RemoveMethod) {
                            CheckMethodIncludeTypeOf(esym.RemoveMethod, compilation, true);
                        }
                    }
                    continue;
                }
            }
        }

        private void CheckFieldIncludeTypeOf(IFieldSymbol fsym, Compilation compilation, ref bool fieldIncludeTypeOf, ref bool staticFieldIncludeTypeOf)
        {
            bool existTypeOf = false;
            foreach (var decl in fsym.DeclaringSyntaxReferences) {
                var node = decl.GetSyntax() as CSharpSyntaxNode;
                if (null != node) {
                    var model = compilation.GetSemanticModel(node.SyntaxTree, true);
                    var analysis = new TypeOfAnalysis(model);
                    node.Accept(analysis);
                    if (analysis.HaveTypeOf) {
                        existTypeOf = true;
                        break;
                    }
                }
            }
            if (existTypeOf) {
                if (fsym.IsStatic) {
                    staticFieldIncludeTypeOf = true;
                    GenerateBasicCctor = true;
                } else {
                    fieldIncludeTypeOf = true;
                    GenerateBasicCtor = true;
                }
                FieldIncludeTypeOfs.Add(fsym.Name, fsym);
            }
        }
        private void CheckMethodIncludeTypeOf(IMethodSymbol msym, Compilation compilation, bool setGenerateBasicFlagIfInclude)
        {
            bool existTypeOf = false;
            foreach (var decl in msym.DeclaringSyntaxReferences) {
                var node = decl.GetSyntax() as CSharpSyntaxNode;
                if (null != node) {
                    var model = compilation.GetSemanticModel(node.SyntaxTree, true);
                    var analysis = new TypeOfAnalysis(model);
                    node.Accept(analysis);
                    if (analysis.HaveTypeOf) {
                        existTypeOf = true;
                        break;
                    }
                }
            }
            if (existTypeOf) {
                string manglingName = SymbolTable.CalcMethodMangling(msym);
                MethodIncludeTypeOfs.Add(manglingName, msym);
                if (setGenerateBasicFlagIfInclude) {
                    if (!msym.IsStatic) {
                        GenerateBasicCtor = true;
                        GenerateTypeParamFields = true;
                    }
                }
            }
        }
    }
    internal class SymbolTable
    {
        internal IAssemblySymbol AssemblySymbol
        {
            get { return m_AssemblySymbol; }
        }
        internal Dictionary<string, INamespaceSymbol> NamespaceSymbols
        {
            get { return m_NamespaceSymbols; }
        }
        internal Dictionary<string, ClassSymbolInfo> ClassSymbols
        {
            get { return m_ClassSymbols; }
        }
        internal Dictionary<string, HashSet<string>> Requires
        {
            get { return m_Requires; }
        }
        internal void AddRequire(string refClass, string moduleName)
        {
            HashSet<string> hashset;
            if (!m_Requires.TryGetValue(refClass, out hashset)) {
                hashset = new HashSet<string>();
                m_Requires.Add(refClass, hashset);
            }
            if (!hashset.Contains(moduleName)) {
                hashset.Add(moduleName);
            }
        }
        internal string NameMangling(IMethodSymbol sym)
        {
            string ret = sym.Name;
            if (ret[0] == '.')
                ret = ret.Substring(1);
            string key = ClassInfo.CalcTypeReference(sym.ContainingType);
            ClassSymbolInfo csi;
            if (m_ClassSymbols.TryGetValue(key, out csi)) {
                bool isMangling;
                csi.SymbolOverloadFlags.TryGetValue(ret, out isMangling);
                if (isMangling) {
                    ret = CalcMethodMangling(sym);
                }
            }
            return ret;
        }
        internal bool ExistTypeOf(IFieldSymbol sym)
        {
            bool ret = false;
            string key = ClassInfo.CalcTypeReference(sym.ContainingType);
            ClassSymbolInfo csi;
            if (m_ClassSymbols.TryGetValue(key, out csi)) {
                ret = csi.FieldIncludeTypeOfs.ContainsKey(sym.Name);
            }
            return ret;
        }
        internal bool ExistTypeOf(IMethodSymbol sym)
        {
            bool ret = false;
            string key = ClassInfo.CalcTypeReference(sym.ContainingType);
            ClassSymbolInfo csi;
            if (m_ClassSymbols.TryGetValue(key, out csi)) {
                string manglingName = CalcMethodMangling(sym);
                ret = csi.MethodIncludeTypeOfs.ContainsKey(manglingName);
            }
            return ret;
        }
        internal SymbolTable(CSharpCompilation compilation)
        {
            m_Compilation = compilation;
            Init(compilation.Assembly);
        }

        private void Init(IAssemblySymbol assemblySymbol)
        {
            m_AssemblySymbol = assemblySymbol;
            INamespaceSymbol nssym = m_AssemblySymbol.GlobalNamespace;
            InitRecursively(nssym);
        }
        private void InitRecursively(INamespaceSymbol nssym)
        {
            if (null != nssym) {
                string ns = ClassInfo.GetNamespaces(nssym);
                m_NamespaceSymbols.Add(ns, nssym);
                foreach (var typeSym in nssym.GetTypeMembers()) {
                    InitRecursively(typeSym);
                }
                foreach (var newSym in nssym.GetNamespaceMembers()) {
                    InitRecursively(newSym);
                }
            }
        }
        private void InitRecursively(INamedTypeSymbol typeSym)
        {
            ClassSymbolInfo csi = new ClassSymbolInfo();
            csi.Init(typeSym, m_Compilation);
            m_ClassSymbols.Add(csi.ClassKey, csi);
            foreach (var newSym in typeSym.GetTypeMembers()) {
                InitRecursively(newSym);
            }
        }

        private CSharpCompilation m_Compilation = null;
        private IAssemblySymbol m_AssemblySymbol = null;
        private Dictionary<string, INamespaceSymbol> m_NamespaceSymbols = new Dictionary<string, INamespaceSymbol>();
        private Dictionary<string, ClassSymbolInfo> m_ClassSymbols = new Dictionary<string, ClassSymbolInfo>();
        private Dictionary<string, HashSet<string>> m_Requires = new Dictionary<string, HashSet<string>>();

        internal static string CalcMethodMangling(IMethodSymbol methodSym)
        {
            StringBuilder sb = new StringBuilder();
            string name = methodSym.Name;
            if (name[0] == '.')
                name = name.Substring(1);
            sb.Append(name);
            foreach (var param in methodSym.Parameters) {
                sb.Append("__");
                if (param.RefKind == RefKind.Ref) {
                    sb.Append("Ref_");
                } else if (param.RefKind == RefKind.Out) {
                    sb.Append("Out_");
                }
                if (param.Type.Kind == SymbolKind.ArrayType) {
                    sb.Append("Arr_");
                    var arrSym = param.Type as IArrayTypeSymbol;
                    string fn = ClassInfo.GetFullNameWithTypeArguments(arrSym.ElementType);
                    sb.Append(fn.Replace('.', '_'));
                } else {
                    string fn = ClassInfo.GetFullNameWithTypeArguments(param.Type);
                    sb.Append(fn.Replace('.', '_'));
                }
            }
            return sb.ToString();
        }
    }
}
