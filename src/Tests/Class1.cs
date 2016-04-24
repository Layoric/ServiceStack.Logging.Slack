using System.Threading;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Logging.Slack;

namespace Tests
{
    [TestFixture]
    public class UnitTests
    {
        AppSelfHostBase testAppHost = new TestAppHost();

        [OneTimeSetUp]
        public void SetUp()
        {
            testAppHost.Init().Start("http://localhost:22334/");
        }

        [Test]
        public void CanLogWithoutChannel()
        {
            LogManager.LogFactory = new SlackLogFactory("http://localhost:22334/testing");
            TestAppHost.AssertCallback = message =>
            {
                Assert.That(message.Channel, Is.EqualTo(null));
                Assert.That(message.Text, Is.EqualTo("This is a test"));
            };
            LogManager.LogFactory.GetLogger(typeof(TestAppHost)).Error("This is a test");
            // Log is async HTTP request
            Thread.Sleep(10);
        }

        [Test]
        public void CanLogWithDefaultChannel()
        {
            LogManager.LogFactory = new SlackLogFactory("http://localhost:22334/testing")
            {
                DefaultChannel = "Testing"
            };
            TestAppHost.AssertCallback = message =>
            {
                Assert.That(message.Channel, Is.EqualTo("Testing"));
                Assert.That(message.Text, Is.EqualTo("This is a test"));
            };
            LogManager.LogFactory.GetLogger(typeof(TestAppHost)).Error("This is a test");
            // Log is async HTTP request
            Thread.Sleep(10);
        }
    }
}
