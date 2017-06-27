using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core
{
	public interface ICompiler
	{
		Type ContexType { get; }
		MethodBuilder MethodBuilder { get; }
		ICompiler Create();
	}
}