using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core.SyntaxUnits
{
    internal class SfsNotTag : SfsTag
    {
        public SfsNotTag(string name, IEnumerable<SfsSyntaxUnit> values)
            : base(name, values)
        {
        }

        public SfsNotTag(IEnumerable<string> ids, IEnumerable<SfsSyntaxUnit> values)
            : base(ids, values)
        {
        }

        public override string Reduce(object context)
        {
            var values = Values as SfsSyntaxUnit[] ?? Values.ToArray();
            var namedValue = GetNamedValue(context, Ids as string[] ?? Ids.ToArray());
            if (namedValue is bool)
            {
                var b = !(bool) namedValue;
                if (b)
                {
                    return values[0].Reduce(context);
                }
                return values.Length > 1 ? values[1].Reduce(context) : string.Empty;
            }
            if (namedValue is string)
            {
                var b = string.IsNullOrEmpty((string) namedValue);
                if (b)
                {
                    return values[0].Reduce(context);
                }
                return values.Length > 1 ? values[1].Reduce(context) : string.Empty;
            }

            if (namedValue == null)
            {
                return values[0].Reduce(context);
            }
            return values.Length > 1 ? values[1].Reduce(context) : string.Empty;
        }
    }
}