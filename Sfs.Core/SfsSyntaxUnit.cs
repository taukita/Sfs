using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core
{
    public abstract class SfsSyntaxUnit
    {
	    public abstract void Accept(ICompiler compiler);

		public HashSet<string> CollectIds()
		{
			var ids = new HashSet<string>();
			CollectIds(ids);
			return ids;
		}

		public abstract string Reduce(object context);

	    protected internal abstract void CollectIds(HashSet<string> ids);

		protected static object GetNamedValue(object context, string[] ids)
        {
            while (true)
            {
                var namedValue = context.GetType().GetProperty(ids.First()).GetValue(context);
                if (ids.Length == 1)
                {
                    return namedValue;
                }
                context = namedValue;
                ids = ids.Skip(1).ToArray();
            }
        }
    }
}
