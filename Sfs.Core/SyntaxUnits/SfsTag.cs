using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
            var labels = new List<Label>();
            var methods = new List<MethodBuilder>();

            var generator = compiler.MethodBuilder.GetILGenerator();

            foreach (var unit in Values)
            {
                var subCompiler = compiler.Create();
                unit.Accept(subCompiler);
                labels.Add(generator.DefineLabel());
                methods.Add(subCompiler.MethodBuilder);
            }

            var type = LoadContextField(generator, Ids, compiler.ContexType);
            generator.Emit(OpCodes.Stloc_0);

            generator.DeclareLocal(type);
            generator.DeclareLocal(typeof(int));            

            BoolBranch(generator, methods);
            EnumBranch(generator, labels, methods);
            StringBranch(generator, methods);

            generator.Emit(OpCodes.Ldstr, string.Empty);
            generator.Emit(OpCodes.Ret);
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

        private void BoolBranch(ILGenerator generator, List<MethodBuilder> methods)
        {
            var end = generator.DefineLabel();
            var @false = generator.DefineLabel();

            generator.Emit(OpCodes.Ldloc_0);

            generator.Emit(OpCodes.Isinst, typeof(bool));
            generator.Emit(OpCodes.Brfalse_S, end);

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Unbox_Any, typeof(bool));

            generator.Emit(OpCodes.Brfalse_S, @false);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, methods[0]);
            generator.Emit(OpCodes.Ret);
            generator.MarkLabel(@false);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, methods[1]);//TODO handle case with only one method
            generator.Emit(OpCodes.Ret);

            generator.MarkLabel(end);
        }

        private void EnumBranch(ILGenerator generator, List<Label> labels, List<MethodBuilder> methods)
        {
            var end = generator.DefineLabel();

            generator.Emit(OpCodes.Ldloc_0);

            generator.Emit(OpCodes.Isinst, typeof(Enum));
            generator.Emit(OpCodes.Brfalse_S, end);

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Castclass, typeof(Enum));
            generator.Emit(OpCodes.Callvirt, typeof(Enum).GetMethod("GetTypeCode"));
            generator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(TypeCode) }));
            generator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToInt32", new[] { typeof(object) }));

            generator.Emit(OpCodes.Stloc_1);

            for (var i = 0; i < Values.Count(); i++)
            {
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldloc_1);
                generator.Emit(OpCodes.Bne_Un, labels[i]);

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, methods[i]);

                generator.Emit(OpCodes.Ret);

                generator.MarkLabel(labels[i]);
            }

            generator.MarkLabel(end);
        }

        private void StringBranch(ILGenerator generator, List<MethodBuilder> methods)
        {
            var end = generator.DefineLabel();
            var @true = generator.DefineLabel();

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Isinst, typeof(string));
            generator.Emit(OpCodes.Brfalse_S, end);

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Castclass, typeof(string));
            generator.Emit(OpCodes.Call, typeof(string).GetMethod("IsNullOrEmpty"));

            generator.Emit(OpCodes.Brtrue_S, @true);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, methods[0]);
            generator.Emit(OpCodes.Ret);
            generator.MarkLabel(@true);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, methods[1]);//TODO handle case with only one method
            generator.Emit(OpCodes.Ret);

            generator.MarkLabel(end);
        }
    }
}