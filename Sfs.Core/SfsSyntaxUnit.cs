using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core
{
    public abstract class SfsSyntaxUnit
    {
        public abstract string Reduce(object context);

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
