using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the sudoku
            Sudoku sudoku = SudokuReader.readFromFile("../../sudokus/9x9.txt", 3);
            Console.WriteLine("Original sudoku:");
            printSudoku(sudoku);
            Console.WriteLine();

            // Test our code
            testDepthFirstSearch(sudoku);

            // Wait for the user
            Console.WriteLine("Press <enter> to quit.");
            Console.ReadLine();
        }

        static void testLocalSearch(Sudoku sudoku)
        {
            // Solve the sudoku using local search
            LocalSearcher searchMachine = new TabuSearcher();
            Sudoku solution = searchMachine.solve(sudoku);
            if(solution == null)
            {
                Console.WriteLine("The local searcher was not able to solve this sudoku...");
            }
            else if(solution.isSolved())
            {
                Console.WriteLine("The solution after local search:");
                printSudoku(solution);
            }
            else
            {
                Console.WriteLine("ERROR! The solution after local search seems to be wrong:");
                printSudoku(solution);
            }
        }
        
        static void testDepthFirstSearch(Sudoku sudoku)
        {
            // We will solve the sudoku using depth-first search and different constraint strategies
            Dictionary<string, ConstraintsHelperFactory> constraintFactories = new Dictionary<string, ConstraintsHelperFactory>();
            constraintFactories.Add("AC1", new ConstraintsHelperFactory_AC1());
            constraintFactories.Add("AC3", new ConstraintsHelperFactory_AC3());
            constraintFactories.Add("AC3 squares", new ConstraintsHelperFactory_AC3_squares());
            constraintFactories.Add("recursive", new ConstraintsHelperFactory_Recursive());
            constraintFactories.Add("trivial", new ConstraintsHelperFactory_Trivial());

            // We will want to measure the performance of each strategy
            foreach(KeyValuePair<string, ConstraintsHelperFactory> entry in constraintFactories)
            {
                // Initialise the necessary objects
                DepthFirstSearch depthFirstSearch = new DepthFirstSearch(entry.Value);
                Sudoku copy = new Sudoku(sudoku);
                Stopwatch stopWatch = new Stopwatch();
                Console.WriteLine("Applying strategy '" + entry.Key + "'.");

                // Measure the time it takes to clean the sudoku
                stopWatch.Start();
                bool cleaned = depthFirstSearch.clean(copy);
                stopWatch.Stop();
                long cleanTime = stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();
                if(!cleaned)
                {
                    Console.WriteLine("Could not clean the sudoku (" + cleanTime.ToString() + " ms).");
                    Console.WriteLine();
                    continue;
                }
                else
                    Console.WriteLine("Cleaned the sudoku (" + cleanTime.ToString() + " ms).");

                // Measure the time it takes to solve the sudoku
                try
                {
                    stopWatch.Start();
                    Sudoku solved = depthFirstSearch.search(copy);
                    stopWatch.Stop();
                    long searchTime = stopWatch.ElapsedMilliseconds;
                    stopWatch.Reset();
                    if(solved == null)
                        Console.WriteLine("This sudoku seems to be impossible to solve (" + searchTime.ToString() + " ms).");
                    else if(!solved.isSolved())
                        Console.WriteLine("Something went wrong while solving the sudoku (" + searchTime.ToString() + " ms).");
                    else
                        Console.WriteLine("The sudoku was solved (" + searchTime.ToString() + " ms), total: " + (cleanTime + searchTime).ToString() + " ms.");
                }
                catch(Exception exc)
                {
                    Console.WriteLine("Exception: " + exc.Message);
                }
                Console.WriteLine();
            }
        }

        static void printSudoku(Sudoku sudoku)
        {
            // Loop through the rows
            for(int y = 0; y < sudoku.NN; ++y)
            {
                // Write a separator when necessary
                if(y != 0 && y % sudoku.N == 0)
                {
                    for(int x = 0; x < sudoku.NN; ++x)
                    {
                        if(x != 0 && x % sudoku.N == 0)
                            Console.Write('+');
                        Console.Write('-');
                    }
                    Console.Write('\n');
                }

                // Loop through the squares in this row
                for(int x = 0; x < sudoku.NN; ++x)
                {
                    // Write a separator when necessary
                    if(x != 0 && x % sudoku.N == 0)
                        Console.Write('|');

                    // Write an x if there are no possibilities for this square
                    // If there is only one possibility, write that possibility
                    // Otherwise we write a '.'
                    List<int> values = sudoku[x, y];
                    if(values.Count == 0)
                        Console.Write('x');
                    else if(values.Count == 1)
                    {
                        if(values[0] <= 9)
                            Console.Write(values[0]);
                        else
                            Console.Write((char) ('A' + (values[0] - 10)));
                    }
                    else
                        Console.Write('.');
                }

                // Next row
                Console.Write('\n');
            }
        }
    }
}
