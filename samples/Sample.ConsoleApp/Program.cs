using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sample.ConsoleApp
{
    public class OrderInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new ServiceCollection();

            container.AddLogging(x => x.AddConsole());
            container.AddCap(x =>
            {
                //console app does not support dashboard
                
                x.UseMySql("Server=192.168.13.214;Port=3306;Database=captest;Uid=root;Pwd=123456;");
                //x.UseRedisMQ("server=192.168.13.214:6379;password=123456;db=2");
                x.UseRedisMQ(op =>
                {
                    op.Server = "192.168.13.214:6379";
                    op.Password = "123456";
                    op.Db = 2;
                });
            });

            container.AddSingleton<EventSubscriber>();
            container.AddSingleton<EventPublish>();

            var sp = container.BuildServiceProvider();

            sp.GetService<IBootstrapper>().BootstrapAsync(default);

            var eventPublish = sp.GetService<EventPublish>();

            DateTime startTime = DateTime.Today.AddHours(-12);
            Console.WriteLine(startTime);
            DateTime endTime = DateTime.Today;
            Console.WriteLine(endTime);

            eventPublish.SendOrder(new OrderInfo{ Id = Guid.NewGuid(),Name = "测试"});

            while (true)
            {
                string str= Console.ReadLine();
                eventPublish.TestSend(str);
            }

            //Console.ReadLine();
        }


    }
}