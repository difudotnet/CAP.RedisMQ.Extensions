// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewLife.Caching;
using Exception = System.Exception;

namespace DotNetCore.CAP.RedisMQ
{
    public class ConnectionChannelPool : IConnectionChannelPool, IDisposable
    {
        private readonly ILogger<ConnectionChannelPool> _logger;
        private readonly FullRedis _fullRedis;
        private static readonly object SLock = new object();

        public ConnectionChannelPool(
            ILogger<ConnectionChannelPool> logger,
            IOptions<CapOptions> capOptionsAccessor,
            IOptions<RedisMQOptions> optionsAccessor)
        {
            _logger = logger;

            var capOptions = capOptionsAccessor.Value;
            var options = optionsAccessor.Value;

            
            _fullRedis = InitFullRedis(options);

            ServersAddress = $"{options.Server}";

            _logger.LogDebug($"RedisMQ configuration:'Server:{_fullRedis.Server}, Password:{_fullRedis.Password}, Db:{_fullRedis.Db}'");
        }

        private FullRedis InitFullRedis(RedisMQOptions options)
        {
            try
            {
                FullRedis fullRedis = null;
                if (string.IsNullOrEmpty(options.Config))
                {
                    fullRedis = new FullRedis(options.Server, options.Password, options.Db);
                }
                else
                {
                    fullRedis = new FullRedis();
                    fullRedis.Init(options.Config);
                }

                fullRedis.Timeout = options.Timeout;
                return fullRedis;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "初始化Redis失败！");
                Console.WriteLine(e);
                throw;
            }
        }

        public string ServersAddress { get; }

        public void Dispose()
        {
            ((IDisposable)_fullRedis).Dispose();
        }

        FullRedis IConnectionChannelPool.Rent()
        {
            return _fullRedis;
        }
    }
}