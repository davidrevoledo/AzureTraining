using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace ServiceBusSample.Plugins
{
    public class CustomPlugin : ServiceBusPlugin
    {
        public override string Name => "CustomPlugin";

        public override Task<Message> AfterMessageReceive(Message message)
        {
            message.ReplyToSessionId = "SessionReply";

            return base.AfterMessageReceive(message);
        }

        public override Task<Message> BeforeMessageSend(Message message)
        {
            message.SessionId = Guid.NewGuid().ToString("N");

            return base.BeforeMessageSend(message);
        }
    }
}