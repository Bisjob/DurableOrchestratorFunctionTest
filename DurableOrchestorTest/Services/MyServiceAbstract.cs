using DurableOrchestratorTest.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DurableOrchestratorTest.Services
{
    public abstract class MyServiceAbstract<T> : IMyService
        where T : ICommand
    {
        public abstract string Name { get; }

        private static Random rdm = new Random();

        public bool CanHandle(ICommand command)
        {
            return (command is T) && command.AllowedServicesNames.Contains(Name);
        }

        public async Task HandleCommandAsync(ICommand command)
        {
            if (!CanHandle(command))
                throw new Exception("Can not handle command");

            await System.IO.File.AppendAllTextAsync($"D:\\{Name}.txt", $"{DateTime.Now.ToString()} : {command.Parameters}\n");
            await Task.Delay(rdm.Next(2000, 5000));
        }
    }
}
