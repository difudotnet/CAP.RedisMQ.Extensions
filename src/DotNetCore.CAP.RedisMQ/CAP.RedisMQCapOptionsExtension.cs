// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using DotNetCore.CAP.RedisMQ;
using DotNetCore.CAP.Transport;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace DotNetCore.CAP
{
    internal sealed class RedisMQCapOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<RedisMQOptions> _configure;

        public RedisMQCapOptionsExtension(Action<RedisMQOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapMessageQueueMakerService>();
             
            services.Configure(_configure);
            services.AddSingleton<ITransport, RedisMQTransport>();
            services.AddSingleton<IConsumerClientFactory, RedisMQConsumerClientFactory>();
            services.AddSingleton<IConnectionChannelPool, ConnectionChannelPool>();
        }
    }
}