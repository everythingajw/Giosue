using System;
using System.Collections.Generic;
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
                PrintTokens(tokens);
                Console.WriteLine();
                Console.WriteLine("Scanning successful.");
            }
            catch (Exception e)
            {
                PrintTokens(scanner.GetTokens());
                Console.WriteLine();
                Console.WriteLine("An error occurred");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine();
            Console.ReadKey();
        }

        private static IEnumerable<IEnumerable<Token>> GroupTokens(IEnumerable<Token> tokens)
        {
            return tokens.GroupBy(t => t.Line);
        }

        private static void PrintTokens(IEnumerable<Token> tokens)
        {
            PrintTokens(GroupTokens(tokens));
        }

        private static void PrintTokens(IEnumerable<IEnumerable<Token>> tokens)
        {
            foreach (var group in tokens)
            {
                foreach (var token in group)
                {
                    Console.WriteLine(token.ToString());
                }
                Console.WriteLine();
            }
        }
    }
}
