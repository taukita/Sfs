using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Sfs.Compiler.Tests
{
	[TestFixture]
	internal class CompilerTest
	{
		[Test]
		public void CompileContextTest()
		{
			Assembly assembly = Compiler.Compile("MyCompiledAssembly", "{a} {a.b} {b} {b.c.d}");
			CheckTypeWithFields(assembly, "Context", "a", "b");
			CheckTypeWithFields(assembly, "Context_a", "b");
			CheckTypeWithFields(assembly, "Context_b", "c");
			CheckTypeWithFields(assembly, "Context_b_c", "d");
		}

		private void CheckTypeWithFields(Assembly assembly, string typeName, params string[] fieldNames)
		{
			var instance = assembly.CreateInstance(typeName);
			Assert.IsNotNull(instance);
			foreach (var fieldName in fieldNames)
			{
				Assert.IsNotNull(instance.GetType().GetField(fieldName));
			}
		}
	}
}
