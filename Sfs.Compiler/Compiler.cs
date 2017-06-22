using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Sfs.Core;

namespace Sfs.Compiler
{
	internal static class Compiler
	{
		public static AssemblyBuilder Compile(string name, string code)
		{
			var cc = new CompilerContext(name);

			CompileContextType(cc, code.SfsParse());

			return cc.AssemblyBuilder;
		}

		private static void CompileContextType(CompilerContext cc, SfsSyntaxUnit unit)
		{
			var typeBuilder = cc.ModuleBuilder.DefineType("Context", TypeAttributes.Public);
			var node = IdNode.FromIds(unit.CollectIds());
			foreach (var child in node.Children.OrderBy(n => n.Id))
			{
				var type = CompileContextType(cc, new[] { "Context", child.Id}, child);
				typeBuilder.DefineField(child.Id, type, FieldAttributes.Public);
			}
			typeBuilder.CreateType();
		}

		private static Type CompileContextType(CompilerContext cc, IEnumerable<string> path, IdNode node)
		{
			if (!node.Children.Any())
			{
				return typeof (object);
			}

			var aPath = path as string[] ?? path.ToArray();

			var typeBuilder = cc.ModuleBuilder.DefineType(string.Join("_", aPath), TypeAttributes.Public);
			foreach (var child in node.Children.OrderBy(n => n.Id))
			{
				var type = CompileContextType(cc, aPath.Concat(new[] {child.Id}), child);
				typeBuilder.DefineField(child.Id, type, FieldAttributes.Public);
			}

			return typeBuilder.CreateType();
		}

		private class CompilerContext
		{
			public CompilerContext(string name)
			{
				var domain = AppDomain.CurrentDomain;
				var assemblyName = new AssemblyName(name);
				AssemblyBuilder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
				ModuleBuilder = AssemblyBuilder.DefineDynamicModule("MainModule", name + ".dll");
			}

			public AssemblyBuilder AssemblyBuilder { get; }
			public ModuleBuilder ModuleBuilder { get; }
		}
	}
}