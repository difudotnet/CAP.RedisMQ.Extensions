## CAP RedisMQ 队列扩展
CAP 是一个基于 .NET Standard 的 C# 库，它是一种处理分布式事务的解决方案，同样具有 EventBus 的功能，它具有轻量级、易使用、高性能等特点。
想了解更多直接移步：https://github.com/dotnetcore/CAP/blob/master/README.zh-cn.md

话不多讲，也不知道咋讲！直接上代码

### 使用前准备
Nuget安装：DotNetCore.CAP.MySql 版本3.1.0

项目引用：DotNetCore.CAP.RedisMQ 这个也可以自己打包dll或者发布到Nuget上面，注意这里面现在用到的Redis访问组件是：NewLife.Redis，了解详情：https://gitee.com/NewLifeX/NewLife.Redis

### 添加CAP并且配置相关Mysql和Redis

```
services.AddCap(x =>
{
    x.UseDashboard();
    x.UseMySql("Server=192.168.13.214;Port=3306;Database=capwebtest;Uid=root;Pwd=123456;");
    //x.UseRedisMQ("server=192.168.13.214:6379;password=123456;db=2");//默认支持链接字符串
    x.UseRedisMQ(op =>
    {
        op.Server = "192.168.13.214:6379";
        op.Password = "123456";
        op.Db = 2;
    });
});
```

### 发布消息
注入ICapPublisher直接使用即可，添加cap时候已经添加到ioc容器。
PublishAsync 函数 name 对应的就是redis队列的key
```
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
```

### 接受消息
直接使用CapSubscribe特性，名称要跟上面发布的对应即可。普通类需要继承ICapSubscribe，Controller控制器可以直接使用
```
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
```

到此结束，在多说两句，我们公司之前用的是阿里的MNS 官方的SDK，不知道是不是我们用法不到位，调用量起来就会爆本地端口不够用错误。优化了很多遍始终没办法彻底解决所以考虑用Redis，环境基本上都现成的，使用起来也方便，希望对大家有帮助。

感谢大佬们的贡献，
CAP：https://github.com/dotnetcore/CAP
Redis：https://gitee.com/NewLifeX/NewLife.Redis
