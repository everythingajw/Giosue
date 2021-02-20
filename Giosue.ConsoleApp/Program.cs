using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Giosue.Exceptions;

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
                PrettyPrintTokens(tokens);
                Console.WriteLine();
                Console.WriteLine("Scanning successful.");
            }
            catch (UnexpectedCharacterException e)
            {
                PrettyPrintTokens(scanner.GetTokens());
                Console.WriteLine();
                Console.WriteLine(e.Message);
                Console.WriteLine($"Character: {e.UnexpectedCharacter}");
                Console.WriteLine($"Line: {e.Line}");
            }
            catch (Exception e)
            {
                PrettyPrintTokens(scanner.GetTokens());
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

        private static void PrettyPrintTokens(IEnumerable<Token> tokens)
        {
            var stringifiedTokens = tokens.Select(t => (t.Type.ToString(), t?.Lexeme?.ToString() ?? "", t?.Literal?.ToString() ?? "", t.Line.ToString()));

            var columnLengths = new int[]
            {
                stringifiedTokens.Max(t => t.Item1.Length),
                stringifiedTokens.Max(t => t.Item2.Length),
                stringifiedTokens.Max(t => t.Item3.Length),
                stringifiedTokens.Max(t => t.Item4.Length),
            };

            var headers = new string[]
            {
                $"{nameof(Token.Type).PadRight(columnLengths[0])} | ",
                $"{nameof(Token.Lexeme).PadRight(columnLengths[1])} | ",
                $"{nameof(Token.Literal).PadRight(columnLengths[2])} | ",
                $"{nameof(Token.Line).PadRight(columnLengths[3])}"
            };

            foreach (var header in headers)
            {
                Console.Write(header);
            }
            Console.WriteLine();
            Console.WriteLine("".PadLeft(headers.Sum(h => h.Length), '-'));

            foreach (var token in stringifiedTokens)
            {
                Console.Write($"{token.Item1.PadRight(columnLengths[0])} | ");
                Console.Write($"{token.Item2.PadRight(columnLengths[1])} | ");
                Console.Write($"{token.Item3.PadRight(columnLengths[2])} | ");
                Console.Write($"{token.Item4.PadRight(columnLengths[3])}");
                Console.WriteLine();
            }
        }
    }
}
