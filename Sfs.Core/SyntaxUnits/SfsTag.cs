using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core.SyntaxUnits
{
    internal class SfsTag : SfsSyntaxUnit
    {
        protected readonly IEnumerable<string> Ids;
        protected readonly IEnumerable<SfsSyntaxUnit> Values;

        public SfsTag(string name, IEnumerable<SfsSyntaxUnit> values)
        {
            Ids = new[] {name};
            Values = values;
        }

        public SfsTag(IEnumerable<string> ids, IEnumerable<SfsSyntaxUnit> values)
        {
            Ids = ids;
            Values = values;
        }

	    public override void Accept(ICompiler compiler)
	    {
		    throw new NotImplementedException();
	    }

	    public override string Reduce(object context)
        {
            var values = Values as SfsSyntaxUnit[] ?? Values.ToArray();
            var namedValue = GetNamedValue(context, Ids as string[] ?? Ids.ToArray());
            if (values.Length == 0)
            {
                return namedValue.ToString();
            }
            if (namedValue is bool)
            {
                var b = (bool) namedValue;
                if (b)
                {
                    return values[0].Reduce(context);
                }
                return values.Length > 1 ? values[1].Reduce(context) : string.Empty;
            }
            if (namedValue is Enum)
            {
                var i = Convert.ToInt32(Convert.ChangeType(namedValue, ((Enum) namedValue).GetTypeCode()));
                return values[i].Reduce(context);
            }
            if (namedValue is string)
            {
                var b = !string.IsNullOrEmpty((string) namedValue);
                if (b)
                {
                    return values[0].Reduce(context);
                }
                return values.Length > 1 ? values[1].Reduce(context) : string.Empty;
            }

            if (namedValue != null)
            {
                return values[0].Reduce(context);
            }
            return values.Length > 1 ? values[1].Reduce(context) : string.Empty;
        }

	    protected internal override void CollectIds(HashSet<string> ids)
	    {
			ids.Add(string.Join(".", Ids));
		    foreach (var unit in Values)
		    {
			    unit.CollectIds(ids);
		    }
		}
    }
}