using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Ast
{
    internal class Generator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="baseName"></param>
        /// <param name="returnType"></param>
        /// <param name="types"></param>
        public static void Define(string outputDir, string baseName, string returnType, string[] types)
        {
            var path = outputDir + "/" + baseName + ".cs";
            var writer = new StreamWriter(path);

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");
            writer.WriteLine("// ReSharper disable IdentifierTypo");
            writer.WriteLine();
            writer.WriteLine("namespace CodeCrafters.Interpreter;");
            writer.WriteLine();
            writer.WriteLine("internal abstract class " + baseName + " {");

            DefineVisitor(writer, baseName, returnType, types);

            // The AST classes.
            foreach (var type in types)
            {
                var splitType = type.Split(':');
                var className = splitType[0].Trim();
                var fields = splitType[1].Trim();
                DefineType(writer, baseName, className, returnType, fields);
            }

            // The base accept() method.
            //writer.WriteLine();
            writer.WriteLine($"  public abstract {GenerateGenericSignature(returnType)};");

            writer.WriteLine("}");

            writer.Close();
        }

        private static void DefineType(StreamWriter writer, string baseName, string className, string returnType, string fieldList)
        {
            var fields = fieldList.Split(", ");

            var lowerFieldNames = new string[fields.Length];
            for (var i = 0; i < fields.Length; i++)
            {
                var split = fields[i].Split(" ");
                lowerFieldNames[i] = split[1] switch
                {
                    "Operator" => $"{split[0]} @{split[1].ToLower()}",
                    _ => $"{split[0]} {split[1].ToLower()}",
                };
            }

            writer.WriteLine("  public class " + className + "(" + string.Join(", ", lowerFieldNames) + ") : " + baseName);
            writer.WriteLine("  {");

            // Fields.
            for (var i = 0; i < fields.Length; i++)
            {
                var fieldName = lowerFieldNames[i].Split(" ")[1];
                writer.WriteLine("    public " + fields[i] + " { get; set; } = " + fieldName + ";");
            }

            // Visitor pattern.
            writer.WriteLine($"    public override {GenerateGenericSignature(returnType)} => visitor.Visit{className}{baseName}(this);");

            writer.WriteLine("  }");
            writer.WriteLine();
        }

        private static string GenerateGenericSignature(string type)
        {
            var generic = type != "void" ? $"<{type}>" : "";

            return $"{type} Accept{generic}(IVisitor{generic} visitor)";
        }

        private static void DefineVisitor(StreamWriter writer, string baseName, string returnType, string[] types)
        {
            var interfaceType = returnType != "void" ? "<out T>" : "";

            writer.WriteLine($"  public interface IVisitor{interfaceType} {{");
            foreach (var type in types)
            {
                var split = type.Split(":");
                var typeName = split[0].Trim();
                writer.WriteLine($"    {returnType} Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }

            writer.WriteLine("  }");
        }
    }
}
