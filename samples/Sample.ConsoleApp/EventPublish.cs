using System;
using DotNetCore.CAP;

namespace Sample.ConsoleApp
{
    public class EventPublish
    {
        private readonly ICapPublisher _capPublisher;

        public EventPublish(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public void TestSend(string str)
        {
            _capPublisher.PublishAsync("sample.console.showsstring", str);

        }

        public void SendOrder(OrderInfo orderInfo)
        {
            _capPublisher.PublishAsync("orderInfo", orderInfo);
        }
    }
}
