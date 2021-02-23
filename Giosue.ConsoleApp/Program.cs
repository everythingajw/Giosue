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
            var stringifiedTokenFields = tokens.Select(t =>
                new string[]
                {
                    t?.Type.ToString() ?? "",
                    t?.Lexeme?.ToString() ?? "",
                    t?.Literal?.ToString() ?? "",
                    t?.Line.ToString() ?? ""
                }).ToList();

            // Calculate the length for each column
            var columnWidths = 
                // Indexes into each array of fields
                Enumerable.Range(0, stringifiedTokenFields.First().Length)
                // Select the maximum length for each field
                .Select(i => stringifiedTokenFields.Max(tokenFields => tokenFields[i].Length))
                .ToList();

            // Get the column headers
            var headers = new string[]
            {
                $"{nameof(Token.Type)}",
                $"{nameof(Token.Lexeme)}",
                $"{nameof(Token.Literal)}",
                $"{nameof(Token.Line)}"
            };

            for (int i = 0; i < columnWidths.Count; i++)
            {
                columnWidths[i] = Math.Max(columnWidths[i] + 1, headers[i].Length);
            }

            // Add separators for the columns
            for (int i = 0; i < headers.Length - 1; i++)
            {
                headers[i] = $"{headers[i].PadRight(columnWidths[i], ' ')} | ";
            }

            // Write the headers
            foreach (var header in headers)
            {
                Console.Write(header);
            }
            Console.WriteLine();

            // Make a line between the headers and the data
            Console.WriteLine("".PadLeft(headers.Sum(h => h.Length), '-'));

            // Print the tokens
            foreach (var tokenList in stringifiedTokenFields)
            {
                var paddedFields = tokenList.Select((t, i) => t.PadRight(columnWidths[i])).ToArray();
                Console.WriteLine(string.Join(" | ", paddedFields));
            }
        }
    }
}
