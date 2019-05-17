using System;
using System.Collections.Generic;
using System.Text;

namespace procExecutor
{
    public class ExecutorConfig
    {
        public string ProcessId { get; set; }

        public string AssemblyPath { get; set; }

        public string ClassType { get; set; }

        public string MethodName { get; set; }

        public int? TimeOutInMilliSeconds { get; set; }
    }
}
