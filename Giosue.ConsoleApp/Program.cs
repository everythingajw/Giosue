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
        const int MaxStringifiedTokenLength = 50;
        static readonly string TestCodePath = Path.GetFullPath(@"..\..\..\..\..\about-giosue\TestCode\MoveTokenInBuffer.gsu");

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine(TestCodePath);

            using var fileReader = new StreamReader(TestCodePath);
            using var source = new FileSource(fileReader);
            //using var source = new StringSource(File.ReadAllText(TestCodePath));
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
                Console.Error.WriteLine();
                Console.Error.WriteLine(e.GetType());
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine($"Character: '{e.UnexpectedCharacter}'");
                Console.Error.WriteLine($"Line: {e.Line}");
            }
            catch (Exception e)
            {
                // Show any other errors
                PrettyPrintTokens(scanner.GetTokens());
                Console.Error.WriteLine();
                Console.Error.WriteLine("An error occurred");
                Console.Error.WriteLine(e.Message);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void PrettyPrintTokens(IEnumerable<Token> tokens)
        {
            var tokenList = tokens.ToList();

            if (!tokenList.Any())
            {
                Console.Error.WriteLine("No tokens to print.");
                return;
            }

            // Stringify the tokens
            var stringifiedTokenFields = tokenList.Select(t =>
                new string[]
                {
                    t?.Type.ToString() ?? "",
                    t?.Lexeme?.ToString() ?? "",
                    t?.Literal?.ToString() ?? "",
                    t?.Line.ToString() ?? ""
                }.Select(t =>
                {
                    // Trim the token down if it's longer than 50 characters.
                    return t.Length >= MaxStringifiedTokenLength ? $"{t.Substring(0, MaxStringifiedTokenLength - 3)}..." : t;
                }).ToList()).ToList();

            // Calculate the length for each column
            var columnWidths =
                // Indexes into each array of fields
                Enumerable.Range(0, stringifiedTokenFields.First().Count)
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

            // Take the maximum possible length for each column.
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
            Console.WriteLine(new string('-', headers.Sum(h => h.Length)));

            // Print the tokens
            foreach (var tokenFieldList in stringifiedTokenFields)
            {
                var paddedFields = tokenFieldList.Select((t, i) => t.PadRight(columnWidths[i])).ToArray();
                Console.WriteLine(string.Join(" | ", paddedFields));
            }
        }
    }
}
