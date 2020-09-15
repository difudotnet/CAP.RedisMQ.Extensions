using System;

// ReSharper disable once CheckNamespace
namespace DotNetCore.CAP
{
    // ReSharper disable once InconsistentNaming
    public class RedisMQOptions
    {
        /// <summary>
        /// Redis地址
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Redis密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 使用DB，默认0
        /// </summary>
        public int Db { get; set; } = 0;

        /// <summary>
        /// 超时时间，默认15000
        /// </summary>
        public int Timeout { get; set; } = 15000;

        /// <summary>
        /// 链接字符串
        /// </summary>
        public string Config { get; set; }
    }
}
