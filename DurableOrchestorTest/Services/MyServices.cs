using DurableOrchestratorTest.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DurableOrchestratorTest.Services
{
    public class MyServiceA : MyServiceAbstract<CommandA>
    {
        public override string Name => nameof(MyServiceA);
    }

    public class MyServiceB : MyServiceAbstract<CommandA>
    {
        public override string Name => nameof(MyServiceB);
    }

    public class MyServiceC : MyServiceAbstract<CommandB>
    {
        public override string Name => nameof(MyServiceC);
    }
}
