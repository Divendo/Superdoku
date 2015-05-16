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
            Sudoku sudoku = SudokuReader.readFromFile("../../sudokus/25x25.txt", 5);
            Console.WriteLine("Original sudoku:");
            printSudoku(sudoku);
            Console.WriteLine();

            // We will solve the sudoku using depth-first search and different constraint strategies
            Dictionary<string, ConstraintsHelperFactory> constraintFactories = new Dictionary<string, ConstraintsHelperFactory>();
            constraintFactories.Add("AC1", new ConstraintsHelperFactory_AC1());
            constraintFactories.Add("AC3", new ConstraintsHelperFactory_AC3());
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
                Console.WriteLine();
            }

            // Wait for the user
            Console.ReadLine();
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
