using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sfs.Core;

namespace Sfs.Compiler.Tests
{
	[TestFixture]
	internal class IdNodeTest
	{
		private static IdNode Node(string template)
		{
			return IdNode.FromIds(template.SfsParse().CollectIds());
		}

		[Test]
		public void SimpleTest()
		{
			var node = Node("ololo");
			Assert.IsNull(node.Id);
			Assert.AreEqual(0, node.Children.Count);
		}

		[Test]
		public void Test1()
		{
			var node = Node("{a} {b} {c}");
			Assert.AreEqual(3, node.Children.Count);
			foreach (var subNode in new[] {"a", "b", "c"}.Select(id => node.Children.FirstOrDefault(n => n.Id == id)))
			{
				Assert.IsNotNull(subNode);
				Assert.AreEqual(0, subNode.Children.Count);
			}
		}

		[Test]
		public void Test2()
		{
			var node = Node("{a.b} {b.c} {c.a}");
			Assert.AreEqual(3, node.Children.Count);
			foreach (var id in new[] {"a", "b", "c"})
			{
				var subNode = node.Children.FirstOrDefault(n => n.Id == id);
				Assert.IsNotNull(subNode);
				Assert.AreEqual(1, subNode.Children.Count);
				Assert.AreEqual(id == "a" ? "b" : (id == "b" ? "c" : "a"), subNode.Children.First().Id);
			}
		}

		[Test]
		public void Test3()
		{
			var node = Node("{a.a} {a.b} {a.c}");
			Assert.AreEqual(1, node.Children.Count);
			node = node.Children.First();
			Assert.AreEqual(3, node.Children.Count);
		}
	}
}
