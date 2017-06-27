using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core.SyntaxUnits
{
    internal class SfsText : SfsSyntaxUnit
    {
        private readonly string _value;

        public SfsText(string value)
        {
            _value = value;
        }

	    public override void Accept(ICompiler compiler)
	    {
			var generator = compiler.MethodBuilder.GetILGenerator();
			generator.Emit(OpCodes.Ldstr, _value);
			generator.Emit(OpCodes.Ret);
		}

	    public override string Reduce(object context)
        {
            return _value;
        }

	    protected internal override void CollectIds(HashSet<string> ids)
	    {
		    //Do nothing
	    }
    }
}