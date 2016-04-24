using System;
using System.Text;

namespace ServiceStack.Logging.Slack
{
    public class SlackLogFactory : ILogFactory
    {
        public bool IsDebugEnabled { get; set; }
        private readonly string incomingWebHookUrl;

        public string DefaultChannel { get; set; }
        public string IconEmoji { get; set; }

        public string ErrorChannel { get; set; }
        public string DebugChannel { get; set; }
        public string InfoChannel { get; set; }
        public string FatalChannel { get; set; }
        public string WarnChannel { get; set; }
        public string BotUsername { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incomingWebHookUrl">Private URL create by Slack when setting up an Incoming WebHook integration.</param>
        /// <param name="isDebugEnabled"></param>
        public SlackLogFactory(string incomingWebHookUrl, bool isDebugEnabled = false)
        {
            IsDebugEnabled = isDebugEnabled;
            this.incomingWebHookUrl = incomingWebHookUrl;
        }

        public ILog GetLogger(Type type)
        {
            return GetLogger(type.ToString());
        }

        public ILog GetLogger(string typeName)
        {
            return new SlackLog(incomingWebHookUrl,IsDebugEnabled)
            {
                DebugChannel = DebugChannel,
                DefaultChannel = DefaultChannel,
                IconEmoji = IconEmoji,
                InfoChannel = InfoChannel,
                FatalChannel = FatalChannel,
                WarnChannel = WarnChannel,
                ErrorChannel = ErrorChannel
            };
        }
    }

    public class SlackLoggingData
    {
        public string Channel { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public string IconEmoji { get; set; }
    }

    public class SlackLog : ILog
    {
        private readonly string incomingWebHookUrl;
        private readonly bool isDebugEnabled;
        public string DefaultChannel { get; set; }
        public string IconEmoji { get; set; }
        public string BotUsername { get; set; }

        public string ErrorChannel { get; set; }
        public string DebugChannel { get; set; }
        public string InfoChannel { get; set; }
        public string FatalChannel { get; set; }
        public string WarnChannel { get; set; }

        private const string NewLine = "\n";

        public SlackLog(string incomingWebHookUrl, bool isDebugEnabled = false)
        {
            this.incomingWebHookUrl = incomingWebHookUrl;
            this.isDebugEnabled = isDebugEnabled;
            this.ErrorChannel = DefaultChannel;
            this.DebugChannel = DefaultChannel;
            this.InfoChannel = DefaultChannel;
            this.FatalChannel = DefaultChannel;
            this.WarnChannel = DefaultChannel;
        }

        private void LogMessage(SlackLoggingData message)
        {
            this.incomingWebHookUrl.PostJsonToUrlAsync(message);
        }

        private SlackLoggingData BuildMessage(string text, string channel = null)
        {
            return new SlackLoggingData
            {
                Channel = channel ?? DefaultChannel,
                IconEmoji = IconEmoji,
                Text = text,
                Username = BotUsername
            };
        }


        private void Write(object message, Exception execption, string channel = null)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(message);
            if(execption != null)
                sb.Append(NewLine);
            while (execption != null)
            {
                sb.Append("Message: ").Append(execption.Message).Append(NewLine)
                .Append("Source: ").Append(execption.Source).Append(NewLine)
                .Append("Target site: ").Append(execption.TargetSite).Append(NewLine)
                .Append("Stack trace: ").Append(execption.StackTrace).Append(NewLine);

                // Walk the InnerException tree
                execption = execption.InnerException;
            }

            var slackMessage = BuildMessage(sb.ToString(), channel);
            LogMessage(slackMessage);
        }

        public void Debug(object message)
        {
            if (!isDebugEnabled)
                return;
            Write(message,null, DebugChannel);
        }

        public void Debug(object message, Exception exception)
        {
            if (!isDebugEnabled)
                return;
            Write(message,exception,DebugChannel);
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (!isDebugEnabled)
                return;
            Write(format.Fmt(args),null,DebugChannel);
        }

        public void Error(object message)
        {
            Write(message,null,ErrorChannel);
        }

        public void Error(object message, Exception exception)
        {
            Write(message,exception,ErrorChannel);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Write(format.Fmt(args),null,ErrorChannel);
        }

        public void Fatal(object message)
        {
            Write(message,null, FatalChannel);
        }

        public void Fatal(object message, Exception exception)
        {
            Write(message,exception,FatalChannel);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Write(format.Fmt(args),null, FatalChannel);
        }

        public void Info(object message)
        {
            Write(message,null, InfoChannel);
        }

        public void Info(object message, Exception exception)
        {
            Write(message,exception,InfoChannel);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Write(format.Fmt(args),null,InfoChannel);
        }

        public void Warn(object message)
        {
            Write(message,null,WarnChannel);
        }

        public void Warn(object message, Exception exception)
        {
            Write(message,exception,WarnChannel);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Write(format.Fmt(args),null,WarnChannel);
        }

        public bool IsDebugEnabled
        {
            get { return isDebugEnabled; }
        }
    }
}
