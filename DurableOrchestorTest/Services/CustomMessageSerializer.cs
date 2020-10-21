using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DurableOrchestratorTest.Services
{
    public class CustomMessageSerializer : IMessageSerializerSettingsFactory
    {
        public JsonSerializerSettings CreateJsonSerializerSettings() => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
        };
    }
}
