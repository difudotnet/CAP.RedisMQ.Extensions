// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.


using NewLife.Caching;

namespace DotNetCore.CAP.RedisMQ
{
    public interface IConnectionChannelPool
    {
        string ServersAddress { get; }

        //string Exchange { get; }

        //IConnection GetConnection();

        FullRedis Rent();

        //bool Return(FullRedis context);
    }
}