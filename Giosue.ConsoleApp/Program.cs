using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Giosue.Exceptions;
using SourceManager;

namespace Giosue.ConsoleApp
{
    class Program
    {
        static readonly string TestCodePath = Path.GetFullPath(@"..\..\..\..\..\about-giosue\TestCode\BasicCode.gsu");

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine(TestCodePath);

            using var fileReader = new StreamReader(TestCodePath);
            using var source = new FileSource(fileReader);
            var scanner = new Scanner(source);

            // This really isn't the most elegant way to handle these errors,
            // but it's easy and rather simple to implement.
            try
            {
                // Try scanning
                var tokens = scanner.ScanTokens();
                
                // Print tokens if successful
                PrettyPrintTokens(tokens);
                Console.WriteLine();
                Console.WriteLine("Scanning successful.");
            }
            catch (UnexpectedCharacterException e)
            {
                // Show any unexpected characters
                PrettyPrintTokens(scanner.GetTokens());
                Console.WriteLine();
                Console.WriteLine(e.Message);
                Console.WriteLine($"Character: '{e.UnexpectedCharacter}'");
                Console.WriteLine($"Line: {e.Line}");
            }
            catch (Exception e)
            {
                // Show any other errors
                PrettyPrintTokens(scanner.GetTokens());
                Console.WriteLine();
                Console.WriteLine("An error occurred");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void PrettyPrintTokens(IEnumerable<Token> tokens)
        {
            // Stringify the tokens
            var stringifiedTokens = tokens.Select(t => (t.Type.ToString(), t?.Lexeme?.ToString() ?? "", t?.Literal?.ToString() ?? "", t.Line.ToString()));

            // Calculate the length for each column
            var columnLengths = new int[]
            {
                stringifiedTokens.Max(t => t.Item1.Length),
                stringifiedTokens.Max(t => t.Item2.Length),
                stringifiedTokens.Max(t => t.Item3.Length),
                stringifiedTokens.Max(t => t.Item4.Length),
            };

            // Get the column headers
            var headers = new string[]
            {
                $"{nameof(Token.Type).PadRight(columnLengths[0])} | ",
                $"{nameof(Token.Lexeme).PadRight(columnLengths[1])} | ",
                $"{nameof(Token.Literal).PadRight(columnLengths[2])} | ",
                $"{nameof(Token.Line).PadRight(columnLengths[3])}"
            };

            // Write the headers
            foreach (var header in headers)
            {
                Console.Write(header);
            }
            Console.WriteLine();

            // Make a line between the headers and the data
            Console.WriteLine("".PadLeft(headers.Sum(h => h.Length), '-'));

            // Print the tokens
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
