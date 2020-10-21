using System;
using System.Collections.Generic;
using System.Text;

namespace DurableOrchestratorTest.Models
{
    public class CommandB : ICommand
    {
        public string Parameters { get; set; }
        public string OtherParameters { get; set; }
        public ICollection<string> AllowedServicesNames { get; set; } = new List<string>();
    }
}
