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
            // Whether or not to test a whole list of sudokus
            bool testMultipleSudokus = true;

            // The type of search mechanism we want to test
            bool testDepthFirstSearchAlgorithm = true;
            
            // Read and test the sudokus
            if(testMultipleSudokus)
            {
                // Read the sudokus
                Sudoku[] sudokus = SudokuReader.readFromFileLines("../../sudokus/norvig-95-9x9.txt", 3);
                Console.WriteLine("{0} sudokus imported.", sudokus.Length);

                // Make sure the SudokuIndexHelper is cached (for fair measurements)
                SudokuIndexHelper.get(sudokus[0].N);

                // Test our code
                if(testDepthFirstSearchAlgorithm)
                    testDepthFirstSearch(sudokus);
                else
                    testLocalSearch(sudokus);
            }
            else
            {
                // Read the sudoku
                Sudoku sudoku = SudokuReader.readFromFile("../../sudokus/9x9.txt", 3);
                Console.WriteLine("Original sudoku:");
                printSudoku(sudoku);
                Console.WriteLine();

                // Make sure the SudokuIndexHelper is cached (for fair measurements)
                SudokuIndexHelper.get(sudoku.N);

                // Test our code
                if(testDepthFirstSearchAlgorithm)
                    testDepthFirstSearch(sudoku);
                else
                    testLocalSearch(sudoku);
            }

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
        
        static void testLocalSearch(Sudoku[] sudokus)
        {
            throw new NotImplementedException();
        }

        static void testDepthFirstSearch(Sudoku sudoku)
        {
            // We will solve the sudoku using depth-first search and different constraint strategies
            Dictionary<string, ConstraintsHelperFactory> constraintFactories = new Dictionary<string, ConstraintsHelperFactory>();
            //constraintFactories.Add("AC1", new ConstraintsHelperFactory_AC1());
            //constraintFactories.Add("AC3", new ConstraintsHelperFactory_AC3());
            //constraintFactories.Add("AC3 squares", new ConstraintsHelperFactory_AC3_squares());
            constraintFactories.Add("recursive", new ConstraintsHelperFactory_Recursive());
            constraintFactories.Add("MAC", new ConstraintsHelperFactory_MAC());
            constraintFactories.Add("trivial", new ConstraintsHelperFactory_Trivial());

            // We will want to measure the performance of each strategy
            foreach(KeyValuePair<string, ConstraintsHelperFactory> entry in constraintFactories)
            {
                for(int useHashMap = 0; useHashMap <= 1; ++useHashMap)
                {
                    // Initialise the necessary objects
                    DepthFirstSearch depthFirstSearch = new DepthFirstSearch(entry.Value);
                    depthFirstSearch.UseHashMap = (useHashMap == 1);
                    Sudoku copy = new Sudoku(sudoku);
                    Stopwatch stopWatch = new Stopwatch();
                    string algorithmName = entry.Key;
                    if(depthFirstSearch.UseHashMap)
                        algorithmName += " (with hashmap)";
                    Console.WriteLine("Applying strategy '" + algorithmName + "'.");

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
        }

        static void testDepthFirstSearch(Sudoku[] sudokus)
        {
            // We will solve the sudoku using depth-first search and different constraint strategies
            Dictionary<string, ConstraintsHelperFactory> constraintFactories = new Dictionary<string, ConstraintsHelperFactory>();
            constraintFactories.Add("AC1", new ConstraintsHelperFactory_AC1());
            constraintFactories.Add("AC3", new ConstraintsHelperFactory_AC3());
            constraintFactories.Add("AC3 squares", new ConstraintsHelperFactory_AC3_squares());
            constraintFactories.Add("recursive", new ConstraintsHelperFactory_Recursive());
            constraintFactories.Add("MAC", new ConstraintsHelperFactory_MAC());
            //constraintFactories.Add("trivial", new ConstraintsHelperFactory_Trivial());

            // Things we are going to measure
            Dictionary<string, long[]> cleanTimes = new Dictionary<string, long[]>();
            Dictionary<string, bool[]> cleaned = new Dictionary<string, bool[]>();
            Dictionary<string, long[]> solveTimes = new Dictionary<string, long[]>();
            Dictionary<string, bool[]> solved = new Dictionary<string, bool[]>();

            // Initialise the measure dictionaries
            foreach(KeyValuePair<string, ConstraintsHelperFactory> entry in constraintFactories)
            {
                cleanTimes.Add(entry.Key, new long[sudokus.Length]);
                cleaned.Add(entry.Key, new bool[sudokus.Length]);
                solveTimes.Add(entry.Key, new long[sudokus.Length]);
                solved.Add(entry.Key, new bool[sudokus.Length]);

                cleanTimes.Add(entry.Key + " (with hashmap)", new long[sudokus.Length]);
                cleaned.Add(entry.Key + " (with hashmap)", new bool[sudokus.Length]);
                solveTimes.Add(entry.Key + " (with hashmap)", new long[sudokus.Length]);
                solved.Add(entry.Key + " (with hashmap)", new bool[sudokus.Length]);
            }

            // Measure the performance on each sudoku
            for(int i = 0; i < sudokus.Length; ++i)
            {
                // Show which sudoku we are solving
                Console.WriteLine("Solving sudoku " + i.ToString());

                // We will want to measure the performance of each strategy
                foreach(KeyValuePair<string, ConstraintsHelperFactory> entry in constraintFactories)
                {
                    for(int useHashMap = 0; useHashMap <= 1; ++useHashMap)
                    {
                        // Initialise the necessary objects
                        DepthFirstSearch depthFirstSearch = new DepthFirstSearch(entry.Value);
                        depthFirstSearch.UseHashMap = (useHashMap == 1);
                        Sudoku copy = new Sudoku(sudokus[i]);
                        Stopwatch stopWatch = new Stopwatch();

                        // Determine the name of the algorithm
                        string algorithmName = entry.Key;
                        if(depthFirstSearch.UseHashMap)
                            algorithmName += " (with hashmap)";

                        // Measure the time it takes to clean the sudoku
                        stopWatch.Start();
                        bool isCleaned = depthFirstSearch.clean(copy);
                        stopWatch.Stop();
                        long cleanTime = stopWatch.ElapsedMilliseconds;
                        stopWatch.Reset();

                        // Recored the measurements
                        cleaned[algorithmName][i] = isCleaned;
                        cleanTimes[algorithmName][i] = cleanTime;

                        // Stop if the sudoku could not be cleaned
                        if(!isCleaned)
                        {
                            Console.WriteLine("Algorithm '" + algorithmName + "' failed to clean the sudoku.");
                            continue;
                        }

                        // Measure the time it takes to solve the sudoku
                        try
                        {
                            stopWatch.Start();
                            Sudoku solvedSudoku = depthFirstSearch.search(copy);
                            stopWatch.Stop();
                            long searchTime = stopWatch.ElapsedMilliseconds;
                            stopWatch.Reset();

                            // Recored the measurements
                            solveTimes[algorithmName][i] = searchTime;
                            if(solvedSudoku == null)
                            {
                                solved[algorithmName][i] = false;
                                Console.WriteLine("Algorithm '" + algorithmName + "' failed to solve the sudoku.");
                            }
                            else if(!solvedSudoku.isSolved())
                            {
                                solved[algorithmName][i] = false;
                                Console.WriteLine("Algorithm '" + algorithmName + "' appeared to have solved the sudoku, but failed.");
                            }
                            else
                            {
                                solved[algorithmName][i] = true;
                                Console.WriteLine("Algorithm '" + algorithmName + "' solved the sudoku.");
                            }
                        }
                        catch(Exception exc)
                        {
                            Console.WriteLine("Algorithm '" + algorithmName + "' threw exception: " + exc.Message);
                        }
                    }
                }

                // Newline
                Console.WriteLine();
            }

            // Calculate and show the stats
            Console.WriteLine("Results (from {0} sudokus):", sudokus.Length);
            Console.WriteLine("--------------------");
            foreach(string entry in cleanTimes.Keys)
            {
                // First we calculate the stats
                long cleanTimeSum = 0;
                long solveTimeSum = 0;
                long wholeTimeSum = 0;
                int cleanCount = 0;
                int solveCount = 0;
                for(int i = 0; i < sudokus.Length; ++i)
                {
                    // We only count cleaned and solved sudokus
                    if(cleaned[entry][i])
                    {
                        ++cleanCount;
                        cleanTimeSum += cleanTimes[entry][i];

                        if(solved[entry][i])
                        {
                            ++solveCount;
                            solveTimeSum += solveTimes[entry][i];
                            wholeTimeSum += cleanTimes[entry][i] + solveTimes[entry][i];
                        }
                    }
                }

                // Then we show them
                Console.WriteLine("Algorithm '" + entry + "' solved {0} sudokus (cleaned {1}).", solveCount, cleanCount);
                Console.WriteLine("Cleaning:\ttotal: {0}ms\tmean: {1}ms", cleanTimeSum, ((double)cleanTimeSum) / cleanCount);
                Console.WriteLine("Solving:\ttotal: {0}ms\tmean: {1}ms", solveTimeSum, ((double)solveTimeSum) / solveCount);
                Console.WriteLine("Total:\t\ttotal: {0}ms\tmean: {1}ms", wholeTimeSum, ((double)wholeTimeSum) / solveCount);
                Console.WriteLine("--------------------");
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
