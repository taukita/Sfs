using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Core
{
    public static class StringExt
    {
        public static string SfsFormat(this string s, object context)
        {
            var unit = SfsParser.ParseAll(s);
            return unit.Reduce(context);
        }

	    public static SfsSyntaxUnit SfsParse(this string s)
	    {
			return SfsParser.ParseAll(s);
		}
    }
}