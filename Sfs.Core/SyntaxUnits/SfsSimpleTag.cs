using System;
using System.Collections.Generic;
using System.Linq;
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

        public override string Reduce(object context)
        {
            var namedValue = GetNamedValue(context, _ids as string[] ?? _ids.ToArray());
            return namedValue == null ? string.Empty : namedValue.ToString();
        }
    }
}
