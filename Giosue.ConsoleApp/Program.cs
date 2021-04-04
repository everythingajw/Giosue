using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Giosue.AST;
using Giosue.Exceptions;
using Giosue.ReturnCodes;
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
            if (args.Length == 0)
            {
                RunREPL();
            } 
            else if (args.Length == 1)
            {
                // Treat it as a path. Run the file.
            }
            else
            {
                // Print usage 
            }
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
            if (scanResult != GiosueReturnCode.AllOK)
            {
                ErrorWriteLine($"Error: {scanResult}");
                ErrorWriteLine($"Code: {(int)scanResult}");
                return (int)scanResult;
            }

            // Scanning successful. Interpret the code.


            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return (int)GiosueReturnCode.AllOK;
        }

        private static GiosueReturnCode RunREPL()
        {

        }

        private static GiosueReturnCode RunFile(string path)
        {
            if (!File.Exists(path))
            {
                ErrorWriteLine("Error: file not found");
                return GiosueReturnCode.FileNotFound;
            }
        }

        private static GiosueReturnCode RunString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return GiosueReturnCode.AllOK;
            }
        }

        private static GiosueReturnCode RunCodeFromSource(Source s)
        {
            var scanResult = ScanCode(s, out var scannedTokens);
            if (scanResult != GiosueReturnCode.AllOK)
            {
                return scanResult;
            }

            var parseResult = ParseCode(scannedTokens, out var parsedExpression);
            if (parseResult != GiosueReturnCode.AllOK)
            {
                return parseResult;
            }

            // For now, just stringify the parsed code.

            return GiosueReturnCode.AllOK;
        }

        private static GiosueReturnCode ScanCode(Source s, out List<Token> scannedTokens)
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
                return GiosueReturnCode.AllOK;
            }
            catch (UnexpectedCharacterException e)
            {
                // Show any unexpected characters
                PrettyPrintTokens(scanner.GetTokens());
                ErrorWriteLine("", e.GetType(), e.Message, $"Character: '{e.UnexpectedCharacter}'", $"Line: {e.Line}");
                return GiosueReturnCode.UnexpectedCharacter;
            }
            catch (UnterminatedStringException e)
            {
                PrettyPrintTokens(scanner.GetTokens());
                ErrorWriteLine("", e.GetType(), e.Message, $"Line: {e.Line}");
                return GiosueReturnCode.UnterminatedString;
            }
            catch (TokenTooLongException e)
            {
                PrettyPrintTokens(scanner.GetTokens());
                ErrorWriteLine("", e.GetType(), e.Message);
                return GiosueReturnCode.TokenTooLong;
            }
            catch (Exception e)
            {
                // Show any other errors
                PrettyPrintTokens(scanner.GetTokens());
                ErrorWriteLine("", "An error occurred", e.Message);
                return GiosueReturnCode.UnknownException;
            }
        }

        private static GiosueReturnCode ParseCode(List<Token> code, out Expression expression)
        {
            expression = default;
            var parser = new Parser(code);

            try
            {
                expression = parser.Parse();
                return GiosueReturnCode.AllOK;
            }
            catch (Exception e)
            {
                ErrorWriteLine(e, e.Message);
                return GiosueReturnCode.UnknownException;
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
