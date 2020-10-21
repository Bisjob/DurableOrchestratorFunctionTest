using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DurableOrchestratorTest.Models
{
    public static class CommandExtensions
    {
        static JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
        };

        public static IEnumerable<ICommand> Split(this ICommand command)
        {
            var results = new List<ICommand>();

            var paramsSplited = command.Parameters.Split(',');

            var setting = new Newtonsoft.Json.JsonSerializerSettings()
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
            };

            foreach (var p in paramsSplited)
            {
                var newCommand = command.Clone();
                newCommand.Parameters = p.Trim();
                results.Add(newCommand);
            }

            return results;
        }

        public static ICommand Clone(this ICommand command)
        {
            var json = JsonConvert.SerializeObject(command, jsonSetting);
            return JsonConvert.DeserializeObject<ICommand>(json, jsonSetting);
        }
    }
}
