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
	internal class CompilerVisitor : ICompiler
	{
		private readonly Counter _counter = new Counter();
		private readonly TypeBuilder _typeBuilder;

		public CompilerVisitor(Type contexType, TypeBuilder typeBuilder)
		{
			ContexType = contexType;
			_typeBuilder = typeBuilder;
			MethodBuilder = typeBuilder.DefineMethod("Generate", MethodAttributes.Public, CallingConventions.Standard,
				typeof (string), new[] {contexType});
		}

		public Type ContexType { get; }
		public MethodBuilder MethodBuilder { get; }

		public ICompiler Create()
		{
			return new SubCompilerVisitor(ContexType, _typeBuilder, _counter);
		}

		private class SubCompilerVisitor : ICompiler
		{
			private readonly Counter _counter;
			private readonly TypeBuilder _typeBuilder;

			public SubCompilerVisitor(Type contexType, TypeBuilder typeBuilder, Counter counter)
			{
				ContexType = contexType;
				_typeBuilder = typeBuilder;
				_counter = counter;
				MethodBuilder = _typeBuilder.DefineMethod("Generate" + _counter.GetNext(), MethodAttributes.Private,
					CallingConventions.Standard, typeof (string), new[] {contexType});
			}

			public Type ContexType { get; }
			public MethodBuilder MethodBuilder { get; }

			public ICompiler Create()
			{
				return new SubCompilerVisitor(ContexType, _typeBuilder, _counter);
			}
		}

		private class Counter
		{
			private int _counter;

			public int GetNext()
			{
				_counter++;
				return _counter;
			}
		}
	}
}