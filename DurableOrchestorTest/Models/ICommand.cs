using System;
using System.Collections.Generic;
using System.Text;

namespace DurableOrchestratorTest.Models
{
    public interface ICommand
    {
        string Parameters { get; set; }

        ICollection<string> AllowedServicesNames { get; set; }

    }
}
