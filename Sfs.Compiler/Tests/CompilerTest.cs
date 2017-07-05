using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Reflection.Emit;

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

		[Test]
		public void CompileGeneratorTest1()
		{
			Assembly assembly = Compiler.Compile("MyCompiledAssembly", "");
			var instance = assembly.CreateInstance("Generator");
			Assert.IsNotNull(instance);
			var mi = instance.GetType().GetMethod("Generate");
			Assert.IsNotNull(mi);
			Assert.AreEqual(string.Empty, mi.Invoke(instance, new object[] {null}));
		}

		[Test]
		public void CompileGeneratorTest2()
		{
			Assembly assembly = Compiler.Compile("MyCompiledAssembly", "абырвалг");
			var instance = assembly.CreateInstance("Generator");
			Assert.IsNotNull(instance);
			var mi = instance.GetType().GetMethod("Generate");
			Assert.IsNotNull(mi);
			Assert.AreEqual("абырвалг", mi.Invoke(instance, new object[] { null }));
		}

		[Test]
		public void CompileGeneratorTest3()
		{
			Assembly assembly = Compiler.Compile("MyCompiledAssembly", "абыр{tag}валг");
			var generator = assembly.CreateInstance("Generator");
			var context = assembly.CreateInstance("Context");
			Assert.IsNotNull(generator);
			Assert.IsNotNull(context);
			var mi = generator.GetType().GetMethod("Generate");
			Assert.IsNotNull(mi);
			Assert.AreEqual("абырвалг", mi.Invoke(generator, new[] { context }));
		}

		[Test]
		public void CompileGeneratorTest4()
		{
			Assembly assembly = Compiler.Compile("MyCompiledAssembly", "{tag}");
			var generator = assembly.CreateInstance("Generator");
			var context = assembly.CreateInstance("Context");
			Assert.IsNotNull(generator);
			Assert.IsNotNull(context);
			context.GetType().GetField("tag").SetValue(context, "абырвалг");
			var mi = generator.GetType().GetMethod("Generate");
			Assert.IsNotNull(mi);
			Assert.AreEqual("абырвалг", mi.Invoke(generator, new[] { context }));
		}

		[Test]
		public void CompileGeneratorTest5()
		{
			Assembly assembly = Compiler.Compile("MyCompiledAssembly", "{tag1}{tag2}");
			var generator = assembly.CreateInstance("Generator");
			var context = assembly.CreateInstance("Context");
			Assert.IsNotNull(generator);
			Assert.IsNotNull(context);
			context.GetType().GetField("tag1").SetValue(context, "абыр");
			context.GetType().GetField("tag2").SetValue(context, "валг");
			var mi = generator.GetType().GetMethod("Generate");
			Assert.IsNotNull(mi);
			Assert.AreEqual("абырвалг", mi.Invoke(generator, new[] { context }));
		}

        [Test]
        public void CompileGeneratorTest6()
        {
            Assembly assembly = Compiler.Compile("MyCompiledAssembly", "{a:1|2|3}");
            var generator = assembly.CreateInstance("Generator");
            var context = assembly.CreateInstance("Context");
            Assert.IsNotNull(generator);
            Assert.IsNotNull(context);
            context.GetType().GetField("a").SetValue(context, Nth.Second);
            var mi = generator.GetType().GetMethod("Generate");
            Assert.IsNotNull(mi);
            Assert.AreEqual("2", mi.Invoke(generator, new[] { context }));
        }

        [Test]
        public void CompileGeneratorTest7()
        {
            Assembly assembly = Compiler.Compile("MyCompiledAssembly", "{a:T|F}");
            var generator = assembly.CreateInstance("Generator");
            var context = assembly.CreateInstance("Context");
            Assert.IsNotNull(generator);
            Assert.IsNotNull(context);            
            var mi = generator.GetType().GetMethod("Generate");
            Assert.IsNotNull(mi);
            context.GetType().GetField("a").SetValue(context, true);
            Assert.AreEqual("T", mi.Invoke(generator, new[] { context }));
            context.GetType().GetField("a").SetValue(context, false);
            Assert.AreEqual("F", mi.Invoke(generator, new[] { context }));
        }

        [Test]
        public void CompileGeneratorTest8()
        {
            Assembly assembly = Compiler.Compile("MyCompiledAssembly", "{a:A|}{b:B|}{c:C|}");
            var generator = assembly.CreateInstance("Generator");
            var context = assembly.CreateInstance("Context");
            Assert.IsNotNull(generator);
            Assert.IsNotNull(context);
            var mi = generator.GetType().GetMethod("Generate");
            Assert.IsNotNull(mi);
            context.GetType().GetField("a").SetValue(context, string.Empty);
            context.GetType().GetField("b").SetValue(context, null);
            context.GetType().GetField("c").SetValue(context, "qwerty");
            Assert.AreEqual("C", mi.Invoke(generator, new[] { context }));
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

        private enum Nth
        {
            First,
            Second,
            Third
        }
	}
}
