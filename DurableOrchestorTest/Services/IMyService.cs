using DurableOrchestratorTest.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DurableOrchestratorTest.Services
{
    public interface IMyService
    {
        string Name { get; }

        bool CanHandle(ICommand command);

        Task HandleCommandAsync(ICommand command);
    }
}
