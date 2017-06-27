using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core.SyntaxUnits
{
    internal class SfsSyntaxUnits : SfsSyntaxUnit
    {
        private readonly IEnumerable<SfsSyntaxUnit> _syntaxUnits;

        public SfsSyntaxUnits(IEnumerable<SfsSyntaxUnit> syntaxUnits)
        {
            _syntaxUnits = syntaxUnits;
        }

	    public override void Accept(ICompiler compiler)
	    {
			var generator = compiler.MethodBuilder.GetILGenerator();
		    if (!_syntaxUnits.Any())
		    {
			    generator.Emit(OpCodes.Ldstr, string.Empty);
		    }
		    else
		    {
			    foreach (var unit in _syntaxUnits)
			    {
				    var subCompiler = compiler.Create();
					unit.Accept(subCompiler);
					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldarg_1);
					generator.Emit(OpCodes.Call, subCompiler.MethodBuilder);
				}
			    if (_syntaxUnits.Count() > 1)
			    {
					var concatMethodInfo = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });
				    for (var i = 0; i < _syntaxUnits.Count() - 1; i++)
				    {
					    generator.Emit(OpCodes.Call, concatMethodInfo);
				    }
				}
		    }
			generator.Emit(OpCodes.Ret);
		}

	    public override string Reduce(object context)
        {
            return string.Join(string.Empty, _syntaxUnits.Select(u => u.Reduce(context)));
        }

	    protected internal override void CollectIds(HashSet<string> ids)
	    {
		    foreach (var unit in _syntaxUnits)
		    {
			    unit.CollectIds(ids);
		    }
	    }
    }
}