using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Funq;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Logging.Slack;

namespace Tests
{
    public class TestAppHost : AppSelfHostBase
    {
        public static Action<SlackMessage> AssertCallback = (req) => { };

        public TestAppHost()
            : base("TestSlackHost", typeof(TestAppHost).Assembly)
        {
            
        }

        public override void Configure(Container container)
        {
            
        }

        [Route("/testing")]
        public class SlackMessage : SlackLoggingData { }

        public class SlackTestService : Service
        {
            public void Any(SlackMessage request)
            {
                TestAppHost.AssertCallback(request);
            }
        }
    }
}
