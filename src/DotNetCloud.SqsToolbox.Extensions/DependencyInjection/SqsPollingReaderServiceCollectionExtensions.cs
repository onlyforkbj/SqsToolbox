﻿using System;
using DotNetCloud.SqsToolbox.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DotNetCloud.SqsToolbox.Extensions.DependencyInjection
{
    public static class SqsPollingReaderServiceCollectionExtensions
    {
        public static IServiceCollection AddPollingSqs(this IServiceCollection services, string queueUrl)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(queueUrl))
            {
                throw new ArgumentNullException(nameof(queueUrl));
            }

            services.TryAddSingleton(new SqsPollingQueueReaderOptions { QueueUrl = queueUrl });

            services.TryAddSingleton<ISqsPollingDelayer, SqsPollingDelayer>();
            services.TryAddSingleton<ISqsPollingQueueReader, SqsPollingQueueReader>();

            return services;
        }


        public static IServiceCollection AddPollingSqs(this IServiceCollection services, Action<SqsPollingQueueReaderOptions> configure)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.Configure(configure);

            services.TryAddSingleton(sp => sp.GetRequiredService<IOptions<SqsPollingQueueReaderOptions>>()?.Value);

            services.TryAddSingleton<ISqsPollingDelayer, SqsPollingDelayer>();
            services.TryAddSingleton<ISqsPollingQueueReader, SqsPollingQueueReader>();

            return services;
        }

        public static IServiceCollection AddPollingSqsBackgroundService(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHostedService<SqsPollingBackgroundService>();

            return services;
        }

        public static IServiceCollection AddPollingSqsBackgroundServiceWithProcessor<T>(this IServiceCollection services) where T : SqsMessageProcessingBackgroundService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHostedService<T>();
            services.AddHostedService<SqsPollingBackgroundService>();

            return services;
        }
    }
}
