// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using DotNetCore.CAP.Transport;
using Microsoft.Extensions.Options;

namespace DotNetCore.CAP.RedisMQ
{
    internal sealed class RedisMQConsumerClientFactory : IConsumerClientFactory
    {
        private readonly IConnectionChannelPool _connectionChannelPool;
        private readonly IOptions<RedisMQOptions> _rabbitMQOptions;

        public RedisMQConsumerClientFactory(IOptions<RedisMQOptions> rabbitMQOptions, IConnectionChannelPool channelPool)
        {
            _rabbitMQOptions = rabbitMQOptions;
            _connectionChannelPool = channelPool;
        }

        public IConsumerClient Create(string groupId)
        {
            try
            {
               var client = new RedisMQConsumerClient(groupId, _connectionChannelPool, _rabbitMQOptions);
               return client;
            }
            catch (System.Exception e)
            {
                throw new BrokerConnectionException(e);
            }
        }
    }
}