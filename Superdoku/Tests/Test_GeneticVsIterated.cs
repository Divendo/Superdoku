using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Note, this class will export more than one file. The provided filename will be used as a prefix.</summary>
    class Test_GeneticVsIterated : Test
    {
        /// <summary>The default max iterations.</summary>
        public const int DEFAULT_MAX_ITERATIONS = 10000;

        /// <summary>The default max iterations without improvement.</summary>
        public const int DEFAULT_MAX_ITERATIONS_WITHOUT_IMPROVEMENT = -1;

        /// <summary>The maximum amount of iterations for the local searcher that is used inside our ILS and GLS.</summary>
        public const int DEFAULT_MAX_ITERATIONS_INNER_SEARCH = -1;

        /// <summary>The maximum amount of iterations without improvement for the local searcher that is used inside our ILS and GLS.</summary>
        public const int DEFAULT_MAX_ITERATIONS_WITHOUT_IMPROVEMENT_INNER_SEARCH = 10;

        public override void runTest(Sudoku[] sudokus, string filename = null)
        {
            // We will solve the sudoku using different parameters for cga simulated annealing
            Dictionary<string, LocalSearcherSwapCounter> localSearchers = new Dictionary<string, LocalSearcherSwapCounter>();
            for(int size = 2; size <= 30; size += 2)
            {
                localSearchers.Add("GLS-" + size.ToString(),
                    new GeneticLocalSearcher(new IterativeSearcher(DEFAULT_MAX_ITERATIONS_INNER_SEARCH, DEFAULT_MAX_ITERATIONS_WITHOUT_IMPROVEMENT_INNER_SEARCH),
                        DEFAULT_MAX_ITERATIONS, DEFAULT_MAX_ITERATIONS_WITHOUT_IMPROVEMENT, size));
                localSearchers.Add("ILS-" + size.ToString(),
                    new IteratedLocalSearch(new IterativeSearcher(DEFAULT_MAX_ITERATIONS_INNER_SEARCH, DEFAULT_MAX_ITERATIONS_WITHOUT_IMPROVEMENT_INNER_SEARCH),
                        DEFAULT_MAX_ITERATIONS, DEFAULT_MAX_ITERATIONS_WITHOUT_IMPROVEMENT, size));
            }

            // Things we are going to measure
            Dictionary<string, long[]> solveTimes = new Dictionary<string, long[]>();
            Dictionary<string, bool[]> solved = new Dictionary<string, bool[]>();
            Dictionary<string, long[]> iterations = new Dictionary<string, long[]>();
            Dictionary<string, long[]> swaps = new Dictionary<string, long[]>();

            // Initialise the measure dictionaries
            foreach(KeyValuePair<string, LocalSearcherSwapCounter> entry in localSearchers)
            {
                solveTimes.Add(entry.Key, new long[sudokus.Length]);
                solved.Add(entry.Key, new bool[sudokus.Length]);
                iterations.Add(entry.Key, new long[sudokus.Length]);
                swaps.Add(entry.Key, new long[sudokus.Length]);
            }

            // Measure the performance on each sudoku
            for(int i = 0; i < sudokus.Length; ++i)
            {
                // Show which sudoku we are solving
                Console.WriteLine("Solving sudoku " + i.ToString());

                // We will want to measure the performance of each strategy
                foreach(KeyValuePair<string, LocalSearcherSwapCounter> entry in localSearchers)
                {
                    // Initialise the necessary objects
                    LocalSearcherSwapCounter localSearcher = entry.Value;
                    Stopwatch stopWatch = new Stopwatch();

                    // Determine the name of the algorithm
                    string algorithmName = entry.Key;

                    // Measure the time it takes to solve the sudoku
                    try
                    {
                        stopWatch.Start();
                        localSearcher.solve(sudokus[i]);
                        stopWatch.Stop();
                        long searchTime = stopWatch.ElapsedMilliseconds;
                        stopWatch.Reset();

                        // Record the measurements
                        solveTimes[algorithmName][i] = searchTime;
                        solved[algorithmName][i] = localSearcher.Solution.toSudoku().isSolved();
                        iterations[algorithmName][i] = localSearcher.Iterations;
                        swaps[algorithmName][i] = localSearcher.TotalSwaps;

                        // Show that this algorithm is done
                        Console.WriteLine("Algorithm '" + algorithmName + "' done.");
                    }
                    catch(Exception exc)
                    {
                        Console.WriteLine("Algorithm '" + algorithmName + "' threw exception: " + exc.Message);
                    }
                }

                // Newline
                Console.WriteLine();
            }

            // Calculate and show the stats
            Console.WriteLine("Results (from {0} sudokus):", sudokus.Length);
            Console.WriteLine("--------------------");
            foreach(string entry in solveTimes.Keys)
            {
                // First we calculate the stats
                SortedDictionary<long, int> cumSolveTime = new SortedDictionary<long, int>();
                SortedDictionary<long, int> cumSwapAmount = new SortedDictionary<long, int>();
                cumSolveTime.Add(0, 0);
                cumSwapAmount.Add(0, 0);
                long maxSolveTime = 0;
                long maxSwapAmount = 0;
                int solveCount = 0;
                for(int i = 0; i < sudokus.Length; ++i)
                {
                    if(!solved[entry][i])
                        continue;

                    ++solveCount;

                    while(maxSolveTime < solveTimes[entry][i])
                    {
                        int value = cumSolveTime[maxSolveTime];
                        maxSolveTime += 100;
                        cumSolveTime.Add(maxSolveTime, value);
                    }
                    for(long solveTime = 100 * (long) Math.Ceiling(solveTimes[entry][i] / 100.0); solveTime <= maxSolveTime; solveTime += 100)
                        ++cumSolveTime[solveTime];

                    while(maxSwapAmount < swaps[entry][i])
                    {
                        int value = cumSwapAmount[maxSwapAmount];
                        maxSwapAmount += 100;
                        cumSwapAmount.Add(maxSwapAmount, value);
                    }
                    for(long swapAmount = 100 * (long)Math.Ceiling(swaps[entry][i] / 100.0); swapAmount <= maxSwapAmount; swapAmount += 100)
                        ++cumSwapAmount[swapAmount];
                }

                // Then we show them
                Console.WriteLine("Algorithm '" + entry + "' solved {0} sudokus.", solveCount);
                Console.WriteLine("--------------------");

                // Export the results
                if(filename != null)
                {
                    // Export the swaps results
                    StreamWriter file = new StreamWriter(filename + "-" + entry + "-swaps.csv");
                    file.WriteLine("swaps;solved");
                    foreach(KeyValuePair<long, int> pair in cumSwapAmount)
                        file.WriteLine("{0};{1}", pair.Key, pair.Value);
                    file.Close();
                    Console.WriteLine("Results have been exported to '{0}'.", filename + "-" + entry + "-swaps.csv");

                    // Export the computation time result
                    file = new StreamWriter(filename + "-" + entry + "-times.csv");
                    file.WriteLine("swaps;solved");
                    foreach(KeyValuePair<long, int> pair in cumSolveTime)
                        file.WriteLine("{0};{1}", pair.Key, pair.Value);
                    file.Close();
                    Console.WriteLine("Results have been exported to '{0}'.", filename + "-" + entry + "-times.csv");
                    Console.WriteLine("--------------------");
                }
            }
        }
    }
}
