// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using DotNetCore.CAP;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class CapOptionsExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static CapOptions UseRedisMQ(this CapOptions options, string config)
        {
            return options.UseRedisMQ(opt => { opt.Config = config; });
        }

        // ReSharper disable once InconsistentNaming
        public static CapOptions UseRedisMQ(this CapOptions options, Action<RedisMQOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new RedisMQCapOptionsExtension(configure));

            return options;
        }
    }
}