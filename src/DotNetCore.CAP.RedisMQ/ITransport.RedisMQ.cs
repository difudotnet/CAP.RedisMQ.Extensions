// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using DotNetCore.CAP.Transport;
using Microsoft.Extensions.Logging;
using NewLife.Caching;
using Newtonsoft.Json;

namespace DotNetCore.CAP.RedisMQ
{
    internal sealed class RedisMQTransport : ITransport
    {
        private readonly IConnectionChannelPool _connectionChannelPool;
        private readonly ILogger _logger;

        public RedisMQTransport(
            ILogger<RedisMQTransport> logger,
            IConnectionChannelPool connectionChannelPool)
        {
            _logger = logger;
            _connectionChannelPool = connectionChannelPool;
        }

        public BrokerAddress BrokerAddress => new BrokerAddress("RedisMQ", _connectionChannelPool.ServersAddress);

        public Task<OperateResult> SendAsync(TransportMessage message)
        {
            FullRedis channel = null;
            try
            {
                channel = _connectionChannelPool.Rent();

                var queue = channel.GetQueue<string>(message.GetName());

                queue.Add(JsonConvert.SerializeObject(message));

                _logger.LogDebug($"RedisMQ topic message [{message.GetName()}] has been published.");

                return Task.FromResult(OperateResult.Success);
            }
            catch (Exception ex)
            {
                var wrapperEx = new PublisherSentFailedException(ex.Message, ex);
                var errors = new OperateError
                {
                    Code = ex.HResult.ToString(),
                    Description = ex.Message
                };

                return Task.FromResult(OperateResult.Failed(wrapperEx, errors));
            }
            // finally
            // {
            //     if (channel != null)
            //     {
            //         //var returned = _connectionChannelPool.Return(channel);
            //         if (!returned)
            //         {
            //             channel.Dispose();
            //         }
            //     }
            // }
        }
    }
}