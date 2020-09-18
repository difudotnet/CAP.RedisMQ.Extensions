// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP.Messages;
using DotNetCore.CAP.Transport;
using Microsoft.Extensions.Options;
using NewLife.Caching;
using Newtonsoft.Json;
using Headers = DotNetCore.CAP.Messages.Headers;

namespace DotNetCore.CAP.RedisMQ
{
    internal sealed class RedisMQConsumerClient : IConsumerClient
    {
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        private readonly IConnectionChannelPool _connectionChannelPool;
        private readonly string _queueName;
        private readonly RedisMQOptions _redisMQOptions;
        private readonly FullRedis _channel;
        private readonly IList<RedisReliableQueue<string>> _redisReliableQueues;


        public RedisMQConsumerClient(string queueName,
            IConnectionChannelPool connectionChannelPool,
            IOptions<RedisMQOptions> options)
        {
             _queueName = queueName;
            _connectionChannelPool = connectionChannelPool;
            _redisMQOptions = options.Value;
            _channel = connectionChannelPool.Rent();
            _redisReliableQueues = new List<RedisReliableQueue<string>>();
        }

        public event EventHandler<TransportMessage> OnMessageReceived;

        public event EventHandler<LogMessageEventArgs> OnLog;

        public BrokerAddress BrokerAddress => new BrokerAddress("RedisMQ", _redisMQOptions.Server);

        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            foreach (var topic in topics)
            {
                _redisReliableQueues.Add(_channel.GetReliableQueue<string>(topic));
            }
        }

        private void ListeningMq(RedisReliableQueue<string> redisReliableQueue)
        {
            while (true)
            {
                try
                {
                    var queue = redisReliableQueue;
                    var msg = queue.TakeOne(1);
                    if (msg != null)
                    {
                        var message = JsonConvert.DeserializeObject<TransportMessage>(msg);

                        message.Headers.Add(Headers.Group, _queueName);

                        OnMessageReceived?.Invoke(message.GetId(), message);

                        // 确认消息
                        queue.Acknowledge(new[] { msg });
                    }
                    else
                    {
                        // 没事干，歇一会
                        System.Threading.Tasks.Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    OnLog?.Invoke(this, new LogMessageEventArgs() { LogType = MqLogType.ExceptionReceived, Reason = $"{_queueName}-{redisReliableQueue.Key}-{ex.Message}" });
                    //throw;
                }
            }
        }

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {

            foreach (var redisReliableQueue in _redisReliableQueues)
            {
                Task.Run(() => ListeningMq(redisReliableQueue), cancellationToken);
            }
            // while (true)
            // {
            //     cancellationToken.ThrowIfCancellationRequested();
            //     cancellationToken.WaitHandle.WaitOne(timeout);
            // }
            // ReSharper disable once FunctionNeverReturns
        }


        public void Commit(object sender)
        {
            //_channel.BasicAck((ulong)sender, false);
        }

        public void Reject(object sender)
        {
            //_channel.BasicReject((ulong)sender, true);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            //_connection?.Dispose();
        }


        #region events

        #endregion
    }
}