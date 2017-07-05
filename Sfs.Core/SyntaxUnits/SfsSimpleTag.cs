using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core.SyntaxUnits
{
    internal class SfsSimpleTag : SfsSyntaxUnit
    {
        private readonly IEnumerable<string> _ids;

        public SfsSimpleTag(IEnumerable<string> ids)
        {
            _ids = ids;
        }

	    public override void Accept(ICompiler compiler)
	    {
		    var generator = compiler.MethodBuilder.GetILGenerator();
            LoadContextField(generator, _ids, compiler.ContexType);
		    generator.Emit(OpCodes.Ret);
	    }

	    public override string Reduce(object context)
        {
            var namedValue = GetNamedValue(context, _ids as string[] ?? _ids.ToArray());
            return namedValue?.ToString() ?? string.Empty;
        }

	    protected internal override void CollectIds(HashSet<string> ids)
	    {
		    ids.Add(string.Join(".", _ids));
	    }
    }
}
