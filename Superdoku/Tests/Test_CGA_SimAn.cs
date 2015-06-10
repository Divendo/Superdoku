using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class Test_CGA_SimAn : Test
    {
        /// <summary>The default max iterations.</summary>
        public const int DEFAULT_MAX_ITERATIONS = 1000000;

        /// <summary>The default max iterations without improvement.</summary>
        public const int DEFAULT_MAX_ITERATIONS_WITHOUT_IMPROVEMENT = -1;

        public override void runTest(Sudoku[] sudokus, string filename = null)
        {
            // We will solve the sudoku using different parameters for cga simulated annealing
            Dictionary<string, SimulatedAnnealingCGAHybrid> localSearchers = new Dictionary<string, SimulatedAnnealingCGAHybrid>();
            for(int iterationsWithoutImprovement = 2; iterationsWithoutImprovement <= 20; ++iterationsWithoutImprovement)
                localSearchers.Add("Simulated annealing CGA hybrid (iters = " + iterationsWithoutImprovement.ToString() + ")", new SimulatedAnnealingCGAHybrid(DEFAULT_MAX_ITERATIONS, DEFAULT_MAX_ITERATIONS_WITHOUT_IMPROVEMENT, iterationsWithoutImprovement));

            // Things we are going to measure
            Dictionary<string, long[]> solveTimes = new Dictionary<string, long[]>();
            Dictionary<string, bool[]> solved = new Dictionary<string, bool[]>();
            Dictionary<string, int[]> heuristicValues = new Dictionary<string, int[]>();
            Dictionary<string, long[]> iterations = new Dictionary<string, long[]>();
            Dictionary<string, long[]> iterationsCga = new Dictionary<string, long[]>();
            Dictionary<string, int[]> heuristicValuesCga = new Dictionary<string, int[]>();

            // Initialise the measure dictionaries
            foreach(KeyValuePair<string, SimulatedAnnealingCGAHybrid> entry in localSearchers)
            {
                solveTimes.Add(entry.Key, new long[sudokus.Length]);
                solved.Add(entry.Key, new bool[sudokus.Length]);
                heuristicValues.Add(entry.Key, new int[sudokus.Length]);
                iterations.Add(entry.Key, new long[sudokus.Length]);
                iterationsCga.Add(entry.Key, new long[sudokus.Length]);
                heuristicValuesCga.Add(entry.Key, new int[sudokus.Length]);
            }

            // Measure the performance on each sudoku
            for(int i = 0; i < sudokus.Length; ++i)
            {
                // Show which sudoku we are solving
                Console.WriteLine("Solving sudoku " + i.ToString());

                // We will want to measure the performance of each strategy
                foreach(KeyValuePair<string, SimulatedAnnealingCGAHybrid> entry in localSearchers)
                {
                    // Initialise the necessary objects
                    SimulatedAnnealingCGAHybrid localSearcher = entry.Value;
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
                        heuristicValues[algorithmName][i] = localSearcher.Solution.HeuristicValue;
                        iterations[algorithmName][i] = localSearcher.Iterations;
                        iterationsCga[algorithmName][i] = localSearcher.IterationsSpentOnCga;
                        heuristicValuesCga[algorithmName][i] = localSearcher.HeuristicValueAfterCga;

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

            // Create an exporter
            ResultsExporter resultsExporter = null;
            if(filename != null)
                resultsExporter = new ResultsExporter(filename);

            // Calculate and show the stats
            Console.WriteLine("Results (from {0} sudokus):", sudokus.Length);
            Console.WriteLine("--------------------");
            foreach(string entry in solveTimes.Keys)
            {
                // First we calculate the stats
                long solveTimeSum = 0;
                long heuristicSum = 0;
                int solveCount = 0;
                long iterationSum = 0;
                long iterationSumSolved = 0;
                long iterationsCgaSum = 0;
                long iterationsCgaSumSolved = 0;
                long heuristicValueCgaSum = 0;
                for(int i = 0; i < sudokus.Length; ++i)
                {
                    heuristicSum += heuristicValues[entry][i];
                    iterationSum += iterations[entry][i];
                    iterationsCgaSum += iterationsCga[entry][i];
                    heuristicValueCgaSum += heuristicValuesCga[entry][i];

                    if(solved[entry][i])
                    {
                        ++solveCount;
                        solveTimeSum += solveTimes[entry][i];
                        iterationSumSolved += iterations[entry][i];
                        iterationsCgaSumSolved += iterationsCga[entry][i];
                    }
                }

                // Add the results to the exporter
                if(resultsExporter != null)
                {
                    resultsExporter.addResult(entry, "solve time", solveTimeSum);
                    resultsExporter.addResult(entry, "solve count", solveCount);
                    resultsExporter.addResult(entry, "heuristic value", heuristicSum);
                    resultsExporter.addResult(entry, "heuristic value cga", heuristicValueCgaSum);
                    resultsExporter.addResult(entry, "iteration count", iterationSum);
                    resultsExporter.addResult(entry, "iteration count (solved)", iterationSumSolved);
                    resultsExporter.addResult(entry, "iteration cga count", iterationsCgaSum);
                    resultsExporter.addResult(entry, "iteration cga count (solved)", iterationsCgaSumSolved);
                    resultsExporter.addResult(entry, "sudoku count", sudokus.Length);
                }

                // Then we show them
                Console.WriteLine("Algorithm '" + entry + "' solved {0} sudokus.", solveCount);
                Console.WriteLine("Solving:\t\ttotal: {0}ms\tmean: {1}ms", solveTimeSum, ((double)solveTimeSum) / solveCount);
                Console.WriteLine("Heuristic:\t\ttotal: {0}\tmean: {1}", heuristicSum, ((double)heuristicSum) / sudokus.Length);
                Console.WriteLine("Iterations:\t\ttotal: {0}\tmean: {1}", iterationSum, ((double)iterationSum) / sudokus.Length);
                Console.WriteLine("Iterations (solved):\ttotal: {0}\tmean: {1}", iterationSumSolved, ((double)iterationSumSolved) / solveCount);
                Console.WriteLine("--------------------");
            }

            // Export the results
            if(resultsExporter != null)
            {
                resultsExporter.write();
                Console.WriteLine("Results have been exported to '{0}'.", resultsExporter.Filename);
                Console.WriteLine("--------------------");
            }
        }
    }
}
