using System;
using System.Collections.Generic;
using System.Text;

namespace DurableOrchestratorTest.Models
{
    public class CommandA : ICommand
    {
        public string Parameters { get; set; }
        public ICollection<string> AllowedServicesNames { get; set; }
    }
}
