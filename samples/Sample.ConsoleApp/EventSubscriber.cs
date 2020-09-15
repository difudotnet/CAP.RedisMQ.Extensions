using System;
using DotNetCore.CAP;
using Newtonsoft.Json;

namespace Sample.ConsoleApp
{
    public class EventSubscriber : ICapSubscribe
    {
        [CapSubscribe("sample.console.showtime")]
        public void ShowTime(DateTime date)
        {
            Console.WriteLine(date);
        }

        [CapSubscribe("sample.console.showsstring")]
        public void ShowStr(string date)
        {
            Console.WriteLine(date);
        }
        [CapSubscribe("orderInfo")]
        public void ShowOrder(OrderInfo orderInfo)
        {
            Console.WriteLine(JsonConvert.SerializeObject(orderInfo));

            //throw new Exception("测试错误");
        }
    }
}
