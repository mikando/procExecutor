using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace procExecutor
{
    class Program
    {
        static ExecutorConfig config;
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    throw new Exception("config file is missing");
                }
                if (System.IO.File.Exists(args[0]) == false)
                {
                    throw new Exception("config file not found");
                }
                var content = System.IO.File.ReadAllText(args[0], new UTF8Encoding(false));
                config = JsonConvert.DeserializeObject<ExecutorConfig>(content);
                using (var mutex = new Mutex(true, config.ProcessId, out bool createdNew))
                {
                    if (createdNew)
                    {
                        Console.WriteLine("Executing");
                        var ts = config.TimeOutInMilliSeconds.HasValue ? TimeSpan.FromMilliseconds(config.TimeOutInMilliSeconds.Value) : TimeSpan.FromDays(1);
                        Thread t = new Thread(runSTA);
                        t.Start();
                        if (!t.Join(ts))
                        {
                            t.Abort();
                            Console.WriteLine("Timeout");
                        }
                        else
                        {
                            Console.WriteLine("Executed");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Allready running");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private static void runSTA()
        {
            Task.Run(run).GetAwaiter().GetResult();
        }

        private static async Task run()
        {
            Assembly testAssembly = Assembly.LoadFile(config.AssemblyPath);
            Type classType = testAssembly.GetType(config.ClassType);
            var method = classType.GetMethod(config.MethodName, BindingFlags.Public | BindingFlags.Static);
            await (Task)method.Invoke(null, null);
        }
    }
}
