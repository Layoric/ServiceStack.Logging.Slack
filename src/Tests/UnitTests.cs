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
        readonly AppSelfHostBase testAppHost = new TestAppHost();

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

        [Test]
        public void CanLogWithTypeSpecificChannels()
        {
            LogManager.LogFactory = new SlackLogFactory("http://localhost:22334/testing", true)
            {
                DefaultChannel = "Testing",
                ErrorChannel = "ERROR",
                InfoChannel = "INFO",
                WarnChannel = "WARN",
                DebugChannel = "DEBUG"
            };
            //ERROR
            TestAppHost.AssertCallback = message =>
            {
                Assert.That(message.Channel, Is.EqualTo("ERROR"));
                Assert.That(message.Text, Is.EqualTo("This is a test"));
            };
            LogManager.LogFactory.GetLogger(typeof(TestAppHost)).Error("This is a test");

            //WARN
            TestAppHost.AssertCallback = message =>
            {
                Assert.That(message.Channel, Is.EqualTo("WARN"));
                Assert.That(message.Text, Is.EqualTo("This is a test"));
            };
            LogManager.LogFactory.GetLogger(typeof(TestAppHost)).Warn("This is a test");

            //INFO
            TestAppHost.AssertCallback = message =>
            {
                Assert.That(message.Channel, Is.EqualTo("INFO"));
                Assert.That(message.Text, Is.EqualTo("This is a test"));
            };
            LogManager.LogFactory.GetLogger(typeof(TestAppHost)).Info("This is a test");

            //DEBUG
            TestAppHost.AssertCallback = message =>
            {
                Assert.That(message.Channel, Is.EqualTo("DEBUG"));
                Assert.That(message.Text, Is.EqualTo("This is a test"));
            };
            LogManager.LogFactory.GetLogger(typeof(TestAppHost)).Debug("This is a test");
            // Log is async HTTP request
            Thread.Sleep(10);
        }

        [Test]
        public void DebugNotUsedByDefault()
        {
            LogManager.LogFactory = new SlackLogFactory("http://localhost:22334/testing");
            bool assertNeverFired = true;
            //ERROR
            TestAppHost.AssertCallback = message => assertNeverFired = false;
            LogManager.LogFactory.GetLogger(typeof(TestAppHost)).Debug("This is a test.");
            Thread.Sleep(10);
            Assert.That(assertNeverFired,Is.EqualTo(true));
        }
    }
}
