using DurableOrchestratorTest.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: WebJobsStartup(typeof(DurableOrchestratorTest.Startup))]

namespace DurableOrchestratorTest
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // To allow polymorphics arguments in orchestrator
            builder.Services.AddSingleton<IMessageSerializerSettingsFactory, CustomMessageSerializer>();

            // Add the three services
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMyService, MyServiceA>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMyService, MyServiceB>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMyService, MyServiceC>());
        }
    }
}
