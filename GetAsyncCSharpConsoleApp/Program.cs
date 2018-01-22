using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAsyncCSharpConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var example1 = new FinishBeforeAwait();

            example1.AccessTheWebAsync().Wait();
        }

        public static void WriteToConsole(string message)
        {
            Console.WriteLine(string.Format("{0}:{1}", DateTime.Now.TimeOfDay.ToString(), message));
        }
    }
}
