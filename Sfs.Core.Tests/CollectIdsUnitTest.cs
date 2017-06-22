using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sfs.Core.Tests
{
	[TestClass]
	public class CollectIdsUnitTest
	{
		[TestMethod]
		public void TextWithoutTagsShouldNotContainsIds()
		{
			var ids = "ololo".SfsParse().CollectIds();
			Assert.AreEqual(0, ids.Count);
		}

		[TestMethod]
		public void SimpleTagsTest()
		{
			var ids = "{a} {a.b} {b.c}".SfsParse().CollectIds();
			Assert.AreEqual(3, ids.Count);
			Assert.IsTrue(ids.Contains("a"));
			Assert.IsTrue(ids.Contains("a.b"));
			Assert.IsTrue(ids.Contains("b.c"));
		}

		[TestMethod]
		public void TagsTest()
		{
			var ids = "{gender:He|She} {character.gender:He|She} {room.character.gender:He|She}".SfsParse().CollectIds();
			Assert.AreEqual(3, ids.Count);
			Assert.IsTrue(ids.Contains("gender"));
			Assert.IsTrue(ids.Contains("character.gender"));
			Assert.IsTrue(ids.Contains("room.character.gender"));
		}

		[TestMethod]
		public void NotTagsTest()
		{
			var ids = "{!gender:He|She} {!character.gender:He|She} {!room.character.gender:He|She}".SfsParse().CollectIds();
			Assert.AreEqual(3, ids.Count);
			Assert.IsTrue(ids.Contains("gender"));
			Assert.IsTrue(ids.Contains("character.gender"));
			Assert.IsTrue(ids.Contains("room.character.gender"));
		}

		[TestMethod]
		public void NestedTagsTest()
		{
			var ids = "{!a:{a.b:A|B}} {b:{c:{!a.b.c:A|B}}}".SfsParse().CollectIds();
			Assert.AreEqual(5, ids.Count);
			Assert.IsTrue(ids.Contains("a"));
			Assert.IsTrue(ids.Contains("a.b"));
			Assert.IsTrue(ids.Contains("a.b.c"));
			Assert.IsTrue(ids.Contains("b"));
			Assert.IsTrue(ids.Contains("c"));
		}
	}
}
