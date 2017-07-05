using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

        protected static Type LoadContextField(ILGenerator generator, IEnumerable<string> ids, Type type)
        {
            generator.Emit(OpCodes.Ldarg_1);
            foreach (var id in ids)
            {
                var fieldInfo = type.GetField(id);
                generator.Emit(OpCodes.Ldfld, fieldInfo);
                type = fieldInfo.FieldType;
            }
            return type;
        }
    }
}
