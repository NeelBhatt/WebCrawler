using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrawler;
using WebCrawler.Repositories;

namespace WebCrawler_Tests
{
    [TestClass]
    public class UnitTest1
    {
        private Crawler crawler = new Crawler(new ExternalUrlRepository(), new OtherUrlRepository(), new FailedUrlRepository(), new CurrentPageUrlRepository());

        /// <summary>
        /// FixPath Method Test
        /// </summary>
        [TestMethod]
        public void FixPathMethodTest()
        {
            var actualResult = Crawler.FixPath("http://python.org", @"/static/stylesheets/mq.css");

            Assert.AreEqual("http:///static/stylesheets/mq.css", actualResult);
        }

        /// <summary>
        /// ResolveRelativePaths Method Test
        /// </summary>
        [TestMethod]
        public void ResolveRelativePathsMethodTest()
        {
            var actualResult = Crawler.ResolveRelativePaths(@"../styles/ChannelMod.css", "http://web.mit.edu/aboutmit");

            Assert.AreEqual("http://styles/ChannelMod.css", actualResult);
        }

        /// <summary>
        /// PageHasBeenCrawled Method Test
        /// </summary>
        [TestMethod]
        public void PageHasBeenCrawledMethodTest()
        {
            var actualResult = Crawler.PageHasBeenCrawled("http://python.org");

            Assert.IsFalse(actualResult);
        }

        /// <summary>
        /// GetWebText Method Test
        /// </summary>
        [TestMethod]
        public void GetWebTextMethodTest()
        {
            var actualResult = Crawler.GetWebText("http://python.org");

            Assert.IsNotNull(actualResult);
        }
    }
}
