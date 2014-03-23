using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HackerSharpAPI;
using System.Threading.Tasks;

namespace HackerTests
{
    [TestClass]
    public class HackerSharpTests
    {
        [TestMethod]
        public async Task TestNews()
        {
            var hackerSharp = new HackerSharp();
            var items = await hackerSharp.News();

            Assert.AreEqual(30, items.Count);
        }

        [TestMethod]
        public async Task TestNewest()
        {
            var hackerSharp = new HackerSharp();
            var items = await hackerSharp.Newest();

            Assert.AreEqual(30, items.Count);
        }
    }
}
