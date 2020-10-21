using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DurableOrchestratorTest.Models;
using DurableOrchestratorTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableOrchestorTest
{
    public class DurableOrchestratorFunction
    {
        private readonly IEnumerable<IMyService> services;
        private const string OrchestratorFunctionId = "1234567";

        public DurableOrchestratorFunction(IEnumerable<IMyService> services)
        {
            this.services = services;
        }

        [FunctionName(nameof(RunHTTP))]
        public async Task<HttpResponseMessage> RunHTTP(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Create the commands
            var commands = new List<ICommand>()
            {
                new CommandA()
                {
                    Parameters = "A1, A2, A3",
                    AllowedServicesNames = new List<string>()
                    {
                        nameof(MyServiceA),
                        nameof(MyServiceB),
                    }
                },
                new CommandB()
                {
                    Parameters = "B1, B2, B3",
                    AllowedServicesNames = new List<string>()
                    {
                        nameof(MyServiceC),
                    }
                }
            };


            string instanceId = await starter.StartNewAsync(nameof(RunOrchestrator), commands);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }


        [FunctionName(nameof(RunOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            // Get command
            var commands = context.GetInput<IEnumerable<ICommand>>();

            log.LogInformation($"########## Start Main orchestrator with {commands.Count()} commands. (Id = {context.InstanceId}) ##########");

            // Create one sub orchestrator Task by service
            var tasks = new List<Task>();
            foreach (var service in services)
            {
                // Get only the allowed commands for this service
                var allowedCommand = commands.Where(c => service.CanHandle(c));
                if (!allowedCommand.Any())
                    continue;

                var ctx = new SubOrchestratorContext()
                {
                    ServiceName = service.Name,
                    Commands = allowedCommand.ToList()
                };

                Task task = context.CallSubOrchestratorAsync(nameof(RunSubOrchestrator), ctx);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            log.LogInformation($"########## End of Main orchestrator with {commands.Count()} commands. (Id = {context.InstanceId}) ##########");
        }


        [FunctionName(nameof(RunSubOrchestrator))]
        public static async Task RunSubOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            var ctx = context.GetInput<SubOrchestratorContext>();

            logger.LogInformation($"****** {ctx.ServiceName} : Start ******");

            // Execute all the commands
            foreach (var command in ctx.Commands)
            {
                // split the command to reduce the executing time by activity
                // and execute. Can NOT perform some parallelism here
                foreach (var c in command.Split())
                {
                    await context.CallActivityAsync(nameof(RunActivity),
                        new ActivityContext()
                        {
                            ServiceName = ctx.ServiceName,
                            Command = c
                        });
                }
            }
            logger.LogInformation($"****** {ctx.ServiceName} : End ******");
        }


        [FunctionName(nameof(RunActivity))]
        public async Task RunActivity([ActivityTrigger] ActivityContext ctx, ILogger log)
        {
            log.LogInformation($"{ctx.ServiceName} : start activity on {ctx.Command.Parameters}");

            // get the required service
            var service = services.Single(s => s.Name == ctx.ServiceName);

            // execute the command
            await service.HandleCommandAsync(ctx.Command);

            log.LogInformation($"{ctx.ServiceName} : End activity on {ctx.Command.Parameters}");
        }
    }

    public class SubOrchestratorContext
    {
        public string ServiceName { get; set; }
        public ICollection<ICommand> Commands { get; set; }
    }
    public class ActivityContext
    {
        public string ServiceName { get; set; }
        public ICommand Command { get; set; }
    }
}