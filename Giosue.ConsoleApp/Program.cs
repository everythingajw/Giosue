using System;
using System.IO;
using System.Linq;

namespace Giosue.ConsoleApp
{
    class Program
    {
        static readonly string TestCodePath = Path.GetFullPath(@"..\..\..\..\..\about-giosue\TestCode\BasicCode.gsu");

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine(TestCodePath);
            
            var testCode = File.ReadAllText(TestCodePath);
            var scanner = new Scanner(testCode);

            try
            {
                var tokens = scanner.ScanTokens();
                tokens.ForEach(t => Console.WriteLine(t.ToString()));
            }
            catch(Exception e)
            {
                Console.WriteLine("An error occurred");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("Tokens were:");
                scanner.GetTokens().ToList().ForEach(t => Console.WriteLine(t.ToString()));
            }
        }
    }
}
