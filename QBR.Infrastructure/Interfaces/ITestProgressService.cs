using System.Collections.Generic;
using QBR.Infrastructure.Models;

namespace QBR.Infrastructure.Interfaces
{
    public interface ITestProgressService
    {
        void AbortTest();

        void StartTest();

        //List<TestStage> TestStages { get; set; }
    }
}