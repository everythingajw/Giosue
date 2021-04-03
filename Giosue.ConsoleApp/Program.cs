using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Giosue.Exceptions;
using SourceManager;
using SourceManager.Exceptions;

namespace Giosue.ConsoleApp
{
    class Program
    {
        const int MaxStringifiedTokenLength = 50;
        static readonly string TestCodePath = Path.GetFullPath(@"..\..\..\..\TestCode\BasicCode.gsu");

        // TODO: Clean up return codes

        static int Main(string[] args)
        {
            Console.WriteLine($"Test code: {TestCodePath}");

            if (!File.Exists(TestCodePath))
            {
                ErrorWriteLine("Test code not found.");
                return 1;
            }
            else if (Directory.Exists(TestCodePath))
            {
                ErrorWriteLine("Test code exists but is a directory");
                return 1;
            }

            using var fileReader = new StreamReader(TestCodePath);
            using var source = new FileSource(fileReader);
            //using var source = new StringSource(File.ReadAllText(TestCodePath));

            var scanResult = ScanCode(source, out var scannedTokens);
            if (scanResult != 0)
            {
                return scanResult;
            }

            // Scanning successful. Interpret the code.


            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return 0;
        }

        private static int ScanCode(Source s, out List<Token> scannedTokens)
        {
            scannedTokens = default;
            var scanner = new Scanner(s);

            try
            {
                // Try scanning
                scannedTokens = scanner.ScanTokens();

                // Print tokens if successful
                //PrettyPrintTokens(tokens);
                Console.WriteLine();
                Console.WriteLine("Scanning successful.");
                return 0;
            }
            catch (UnexpectedCharacterException e)
            {
                // Show any unexpected characters
                PrettyPrintTokens(scanner.GetTokens());
                ErrorWriteLine("", e.GetType(), e.Message, $"Character: '{e.UnexpectedCharacter}'", $"Line: {e.Line}");
                return 2;
            }
            catch (UnterminatedStringException e)
            {
                PrettyPrintTokens(scanner.GetTokens());
                ErrorWriteLine("", e.GetType(), e.Message, $"Line: {e.Line}");
                return 3;
            }
            catch (TokenTooLongException e)
            {
                PrettyPrintTokens(scanner.GetTokens());
                ErrorWriteLine("", e.GetType(), e.Message);
                return 4;
            }
            catch (Exception e)
            {
                // Show any other errors
                PrettyPrintTokens(scanner.GetTokens());
                ErrorWriteLine("", "An error occurred", e.Message);
                return 1;
            }
        }

        /// <summary>
        /// Writes a <see cref="string"/> to <see cref="Console.Error"/> with a newline.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to write.</param>
        private static void ErrorWriteLine(string s = "")
        {
            Console.Error.WriteLine(s);
        }

        /// <summary>
        /// Writes an <see cref="object"/> to <see cref="Console.Error"/> with a newline.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to print</param>
        private static void ErrorWriteLine(object obj)
        {
            Console.Error.WriteLine(obj);
        }

        /// <summary>
        /// Writes multiple <see cref="object"/>s to <see cref="Console.Error"/> with a newline.
        /// </summary>
        /// <param name="objs">The objects to print</param>
        private static void ErrorWriteLine(params object[] objs)
        {
            foreach (var obj in objs)
            {
                ErrorWriteLine(obj);
            }
        }

        private static List<List<string>> StringifyTokens(IEnumerable<Token> tokens, int? columnLength = null, string continuationString = "...")
        {
            var tokenList = tokens.ToList();

            if (!tokenList.Any())
            {
                return new();
            }

            var stringifiedTokens = tokenList.Select(t =>
                new List<string>()
                {
                    t?.Type.ToString() ?? "",
                    t?.Lexeme?.ToString() ?? "",
                    t?.Literal?.ToString() ?? "",
                    t?.Line.ToString() ?? ""
                }
            ).ToList();

            if (columnLength.HasValue)
            {
                var columnLengthValue = columnLength.Value;
                continuationString ??= "";
                return stringifiedTokens.Select(fields =>
                {
                    return fields
                    .Select(t => t.Length > columnLengthValue ? $"{t.Substring(0, columnLengthValue - continuationString.Length)}{continuationString}" : t)
                    .ToList();
                }).ToList();
            }
            else
            {
                return stringifiedTokens;
            }
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
            var stringifiedTokenFields = StringifyTokens(tokenList, MaxStringifiedTokenLength);

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
            // If a field for a token evaluates to a zero-length string,
            // the column will look weird because the header is larger
            // than the token.
            for (int i = 0; i < columnWidths.Count; i++)
            {
                columnWidths[i] = Math.Max(columnWidths[i] + 1, headers[i].Length);
            }

            // Add separators for the columns
            // Skip the last column so it doesn't look like there's a
            // column after the last column.
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
