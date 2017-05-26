using System;
using CanonicalFormExceptions;

namespace CanonicalForm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This program transforms equations into canonical form.");
            Console.WriteLine("Press Ctrl + C to exit the program");
            Console.WriteLine();
            // File mode
            if (args.Length == 1)
            {
                RunFileMode(args[0]);
            }
            // Interactive mode
            else
            {
                RunInteractiveMode();
            }
        }

        // Runs the program as file mode if an command line argument was provided
        private static void RunFileMode(string inputFilename)
        {
            Console.WriteLine("---------------FILE MODE---------------");
            try
            {
                string outputFileName = new Parser().TransformFileToCanonical(inputFilename);
                Console.WriteLine("File created: " + outputFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: The file could not be transformed - " + e.Message);
            }
            Console.WriteLine();
            Console.WriteLine("Press f to go to interactive mode");
            ConsoleKeyInfo keypress = Console.ReadKey();
            while (keypress.Key != ConsoleKey.F)
            {
                keypress = Console.ReadKey();
            }
            Console.WriteLine();
            RunInteractiveMode();
        }

        // Runs the program as interactive mode by default
        private static void RunInteractiveMode()
        {
            string inputline;
            string outputLine;
            bool invalidInput;
            Console.WriteLine("------------INTERACTIVE MODE------------");
            // Will request input indefinitely until program exit
            while (true)
            {
                inputline = "";
                outputLine = "";
                invalidInput = true;
                inputline = ReadNotEmptyLine();
                while (invalidInput)
                {
                    try
                    {
                        outputLine = new Parser().TransformEquationToCanonical(inputline);
                        invalidInput = false;
                    }
                    catch (InvalidEquationException)
                    {
                        Console.WriteLine("Error: Invalid Input");
                        Console.WriteLine();
                        inputline = ReadNotEmptyLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        Console.WriteLine();
                        inputline = ReadNotEmptyLine();

                    }
                }
                Console.WriteLine("Result: " + outputLine);
                Console.WriteLine();
            }
        }

        private static string ReadNotEmptyLine()
        {
            Console.Write("Enter an equation: ");
            string line = Console.ReadLine();
            // Check for empty input
            while (String.IsNullOrEmpty(line))
            {
                Console.Write("ERROR: No input was given");
                Console.WriteLine();
                Console.Write("Enter an equation: ");
                line = Console.ReadLine();
            }
            return line;
        }
    }
}
