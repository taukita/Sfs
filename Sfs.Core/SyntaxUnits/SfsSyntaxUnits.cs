using System;
using System.Collections.Generic;
using System.Linq;
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

        public override string Reduce(object context)
        {
            return string.Join("", _syntaxUnits.Select(u => u.Reduce(context)));
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