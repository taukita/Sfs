using System;
using System.Collections.Generic;
using System.Linq;
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

        public override string Reduce(object context)
        {
            return _value;
        }
    }
}