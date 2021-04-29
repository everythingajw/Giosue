using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private static Environment OldEnvironment;

        // TODO: Clean up return codes

        static int Main(string[] args)
        {
            var returnCode = GiosueExceptionCategory.AllOK;
            var src = @"
ScriveLina(1 + 1 == 2);
ScriveLina(1 + 1 == 2.0);
ScriveLina(1 - 1 == 0);
ScriveLina(1 - 1 == 0.0);
ScriveLina(1 > 2);
ScriveLina(1 < 2);
--ScriveLina(1 + 3 > 2.0);
--ScriveLina(1 + 3 < 2.0);
ScriveLina(TipoDiDato(1+1));
ScriveLina(TipoDiDato(1));
ScriveLina(TipoDiDato(2));";
            try
            {
                //if (args.Length == 0)
                //{
                //    returnCode = RunREPL();
                //}
                //else if (args.Length == 1)
                //{
                //    returnCode = RunFile(args[0]);
                //}
                //else
                //{
                //    ErrorWriteLine("Usage: giosue.exe [path-to-file]");
                //    returnCode = GiosueExceptionCategory.Unknown;
                //}
                Console.WriteLine(src);
                RunString(src);
            }
            catch (Exception e)
            {
                var thisMethod = MethodBase.GetCurrentMethod();
                ErrorWriteLine($"{thisMethod.DeclaringType.FullName}.{thisMethod.Name} :: Exception occurred somewhere. Good luck finding it!");
                ErrorWriteLine(e);
            }

#if DEBUG
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(); 
#endif
            return (int)returnCode;
        }

        private static GiosueExceptionCategory RunREPL()
        {
            var inputNumber = 1;
            while (true)
            {
                Console.Write($"In [{inputNumber++:00#}]> ");

                // Make sure the prompt is written to the console.
                Console.Out.Flush();

                var input = Console.ReadLine();
                switch (input)
                {
                    case null:
                    case "#q;;":
                    case "#quit;;":
                    case "#exit;;":
                        // Using goto is a bit cleaner here.
                        goto omega;
                    default:
                        break;
                }

                RunString(input);
            }

        omega:
            return GiosueExceptionCategory.AllOK;
        }

        private static GiosueExceptionCategory RunFile(string path)
        {
            if (!File.Exists(path))
            {
                ErrorWriteLine("Error: file not found");

                // Temporary
                // TODO: Source exceptions
                throw new FileNotFoundException();
            }

            using var reader = new StreamReader(path);
            using var source = new FileSource(reader);
            return RunCodeFromSource(source);
        }

        private static GiosueExceptionCategory RunString(string s, Environment environment = null)
        {
            if (string.IsNullOrEmpty(s))
            {
                return GiosueExceptionCategory.AllOK;
            }

            using var source = new StringSource(s);
            return RunCodeFromSource(source);
        }

        private static GiosueExceptionCategory RunCodeFromSource(Source s)
        {
            var scanResult = ScanCode(s, out var scannedTokens);
            if (scanResult != null)
            {
                return scanResult.Category;
            }

            var parseResult = ParseCode(scannedTokens, out var statements);
            if (parseResult != null)
            {
                return parseResult.Category;
            }

            if (statements == null)
            {
                ErrorWriteLine("Statements null");
                return GiosueExceptionCategory.Parser;
            }

            var interpreter = new Interpreter(OldEnvironment);

            try
            {
                interpreter.Interpret(statements);

                return GiosueExceptionCategory.AllOK;
            }
            catch (InterpreterException e)
            {
                var thisMethod = MethodBase.GetCurrentMethod();
                ErrorWriteLine($"{thisMethod.DeclaringType.FullName}.{thisMethod.Name} :: interperter exception");
                ErrorWriteLine($"Type: {e.ExceptionType}", $"Type code: {(int)e.ExceptionType}");
                ErrorWriteLine($"Message: {e.Message}");
                return e.Category;
            }
            catch (EnvironmentException e)
            {
                var thisMethod = MethodBase.GetCurrentMethod();
                ErrorWriteLine($"{thisMethod.DeclaringType.FullName}.{thisMethod.Name} :: environment exception");
                ErrorWriteLine($"Type: {e.ExceptionType}", $"Type code: {(int)e.ExceptionType}");
                ErrorWriteLine($"Message: {e.Message}");
                return e.Category;
            }
            finally
            {
                OldEnvironment = interpreter.Environment;
            }
        }

        private static ScannerException ScanCode(Source s, out List<Token> scannedTokens)
        {
            scannedTokens = default;
            var scanner = new Scanner(s);

            try
            {
                scannedTokens = scanner.ScanTokens();

                // Success; no exceptions occurred.
                return null;
            }
            catch (ScannerException e)
            {
                // PrettyPrintTokens(scanner.GetTokens());
                var thisMethod = MethodBase.GetCurrentMethod();
                ErrorWriteLine($"{thisMethod.DeclaringType.FullName}.{thisMethod.Name} :: scanner exception");
                ErrorWriteLine($"Type: {e.ExceptionType}", $"Type code: {(int)e.ExceptionType}", $"Line: {e.Line}");
                ErrorWriteLine($"Message: {e.Message}");
                return e;
            }
        }

        private static ParserException ParseCode(List<Token> code, out List<Statements.Statement> statements)
        {
            statements = default;
            var parser = new Parser(code);
            try
            {
                return parser.Parse(out statements);
            }
            catch (ParserException e)
            {
                var thisMethod = MethodBase.GetCurrentMethod();
                ErrorWriteLine($"{thisMethod.DeclaringType.FullName}.{thisMethod.Name} :: parser exception");
                ErrorWriteLine($"Type: {e.ExceptionType}", $"Type code: {(int)e.ExceptionType}");
                ErrorWriteLine($"Message: {e.Message}");
                ErrorWriteLine($"Erroneous token: {e.ErroneousToken}");
                return e;
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
