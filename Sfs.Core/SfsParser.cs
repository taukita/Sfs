using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfs.Core.SyntaxUnits;
using Sprache;

namespace Sfs.Core
{
    internal static class SfsParser
    {
        internal static SfsSyntaxUnit ParseAll(string source)
        {
            return Text.Or(AnyTag).Many().End().Select(many => new SfsSyntaxUnits(many)).Parse(source);
        }

        private static IEnumerable<T> Cons<T>(T head, IEnumerable<T> rest)
        {
            yield return head;
            foreach (var item in rest)
                yield return item;
        }

        internal static Parser<string> Id =
            from letter in Parse.Letter.Or(Parse.Char('_')).Once()
            from letters in Parse.LetterOrDigit.Or(Parse.Char('_')).Many()
            select new string(letter.Concat(letters).ToArray());

        internal static Parser<IEnumerable<string>> MultiId =
            from first in Id
            from rest in Parse.Char('.').Then(_ => Id).Many()
            select Cons(first, rest);

        internal static Parser<SfsSyntaxUnit> TextOption =
            Parse.CharExcept(new[] {'|', '{', '}', '~'})
                .Or(Parse.String("~|").Select(_ => '|'))
                .Or(Parse.String("~{").Select(_ => '{'))
                .Or(Parse.String("~}").Select(_ => '}'))
                .Or(Parse.String("~~").Select(_ => '~'))
                .Many()
                .Select(c => new SfsText(new string(c.ToArray())));

        internal static Parser<SfsSyntaxUnit> Option =
            TextOption.Or(Parse.Ref(() => AnyTag)).Many().Select(many => new SfsSyntaxUnits(many));

        internal static Parser<IEnumerable<SfsSyntaxUnit>> Options =
            from first in Option
            from rest in Parse.Char('|').Then(_ => Option).Many()
            select Cons(first, rest);

        internal static Parser<SfsSyntaxUnit> Tag =
            from open in Parse.Char('{')
            from name in MultiId
            from colon in Parse.Char(':')
            from options in Options
            from close in Parse.Char('}')
            select new SfsTag(name, options);

        internal static Parser<SfsSyntaxUnit> NotTag =
            from open in Parse.String("{!")
            from name in MultiId
            from colon in Parse.Char(':')
            from options in Options
            from close in Parse.Char('}')
            select new SfsNotTag(name, options);

        internal static Parser<SfsSyntaxUnit> AnyTag = Tag.Or(NotTag);

        internal static Parser<SfsSyntaxUnit> Text =
            Parse.CharExcept(new[] {'{', '}', '~'})
                .Or(Parse.String("~{").Select(_ => '{'))
                .Or(Parse.String("~}").Select(_ => '}'))
                .Or(Parse.String("~~").Select(_ => '~'))
                .Many()
                .Select(c => new SfsText(new string(c.ToArray())));
    }
}