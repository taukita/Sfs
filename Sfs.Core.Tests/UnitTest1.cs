using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sfs.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {
        enum Nth
        {
            First,
            Second,
            Third
        }

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("ololo", "ololo".SfsFormat(null));
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.AreEqual("{ololo:123}", "~{ololo:123~}".SfsFormat(null));
        }

        [TestMethod]
        public void TestMethod3()
        {
            Assert.AreEqual("dragon's head here", "dragon's {head:head|tail} here".SfsFormat(new { head = true }));
        }

        [TestMethod]
        public void TestMethod4()
        {
            Assert.AreEqual(
                "dragon's horned head here",
                "dragon's {head:{horned:horned |}head|tail} here".SfsFormat(new {head = true, horned = true}));
        }

        [TestMethod]
        public void TestMethod5()
        {
            Assert.AreEqual(
                "dragon's head here",
                "dragon's {head:{horned:horned |}head|tail} here".SfsFormat(new {head = true, horned = false}));
        }

        [TestMethod]
        public void TestMethod6()
        {
            Assert.AreEqual(
                "dragon's head here",
                "dragon's {head:{horned:horned }head|tail} here".SfsFormat(new {head = true, horned = false}));
        }

        [TestMethod]
        public void TestMethod7()
        {
            Assert.AreEqual(
                "dragon's horned head here",
                "dragon's {head:{horned:horned }head|tail} here".SfsFormat(new {head = true, horned = true}));
        }

        [TestMethod]
        public void TestMethod8()
        {
            Assert.AreEqual(
                "third was taken",
                "{nth:first|second|third} was taken".SfsFormat(new {nth = Nth.Third}));
        }

        [TestMethod]
        public void TestMethod10()
        {
            Assert.AreEqual("T", "{a.b:T|F}".SfsFormat(new {a = new {b = true}}));
            Assert.AreEqual("F", "{!a.b:T|F}".SfsFormat(new {a = new {b = true}}));
        }

        [TestMethod]
        public void ShouldTreatEmptyStringAsFalseAndNotEmptyStringAsTrue()
        {

            Assert.AreEqual("T", "{a.b:T|F}".SfsFormat(new {a = new {b = "seventh"}}));
            Assert.AreEqual("F", "{a.b:T|F}".SfsFormat(new {a = new {b = string.Empty}}));
            Assert.AreEqual("T", "{!a.b:T|F}".SfsFormat(new {a = new {b = string.Empty}}));
        }

        [TestMethod]
        public void ShouldTreatNullAsFalseAndNotNullObjectAsTrue()
        {
            Assert.AreEqual("T", "{a.b:T|F}".SfsFormat(new {a = new {b = new object()}}));
            Assert.AreEqual("F", "{a.b:T|F}".SfsFormat(new {a = new {b = (object) null}}));
            Assert.AreEqual("T", "{!a.b:T|F}".SfsFormat(new {a = new {b = (object) null}}));
        }

        [TestMethod]
        public void SimpleInterpolation()
        {
            Assert.AreEqual("interpolation works", "interpolation {result}".SfsFormat(new {result = "works"}));
            Assert.AreEqual(
                "interpolation still works",
                "interpolation still {result.result}".SfsFormat(new { result = new { result = "works" } }));
        }
    }
}
