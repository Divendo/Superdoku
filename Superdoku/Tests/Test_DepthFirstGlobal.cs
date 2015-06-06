using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class to test all depth first searchers together.</summary>
    class Test_DepthFirstGlobal : Test
    {
        public override void runTest(Sudoku[] sudokus, string filename = null)
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
            Dictionary<string, long[]> expandedNodes = new Dictionary<string, long[]>();

            // Initialise the measure dictionaries
            foreach(KeyValuePair<string, ConstraintsHelperFactory> entry in constraintFactories)
            {
                cleanTimes.Add(entry.Key, new long[sudokus.Length]);
                cleaned.Add(entry.Key, new bool[sudokus.Length]);
                solveTimes.Add(entry.Key, new long[sudokus.Length]);
                solved.Add(entry.Key, new bool[sudokus.Length]);
                expandedNodes.Add(entry.Key, new long[sudokus.Length]);

                cleanTimes.Add(entry.Key + " (with hashmap)", new long[sudokus.Length]);
                cleaned.Add(entry.Key + " (with hashmap)", new bool[sudokus.Length]);
                solveTimes.Add(entry.Key + " (with hashmap)", new long[sudokus.Length]);
                solved.Add(entry.Key + " (with hashmap)", new bool[sudokus.Length]);
                expandedNodes.Add(entry.Key + " (with hashmap)", new long[sudokus.Length]);
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

                            // Record the measurements
                            expandedNodes[algorithmName][i] = depthFirstSearch.ExpandedNodes;
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

            // Create an exporter
            ResultsExporter resultsExporter = null;
            if(filename != null)
                resultsExporter = new ResultsExporter(filename);

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
                long expandedNodeCount = 0;
                long expandedNodeCountSolved = 0;
                for(int i = 0; i < sudokus.Length; ++i)
                {
                    // We only count cleaned and solved sudokus
                    if(cleaned[entry][i])
                    {
                        ++cleanCount;
                        cleanTimeSum += cleanTimes[entry][i];
                        expandedNodeCount += expandedNodes[entry][i];

                        if(solved[entry][i])
                        {
                            ++solveCount;
                            solveTimeSum += solveTimes[entry][i];
                            wholeTimeSum += cleanTimes[entry][i] + solveTimes[entry][i];
                            expandedNodeCountSolved += expandedNodes[entry][i];
                        }
                    }
                }

                // Add the results to the exporter
                if(resultsExporter != null)
                {
                    resultsExporter.addResult(entry, "clean time", cleanTimeSum);
                    resultsExporter.addResult(entry, "clean count", cleanCount);
                    resultsExporter.addResult(entry, "solve time", solveTimeSum);
                    resultsExporter.addResult(entry, "solve count", solveCount);
                    resultsExporter.addResult(entry, "total time", wholeTimeSum);
                    resultsExporter.addResult(entry, "expanded nodes", expandedNodeCount);
                    resultsExporter.addResult(entry, "expanded nodes (solved)", expandedNodeCountSolved);
                    resultsExporter.addResult(entry, "sudoku count", sudokus.Length);
                }

                // Then we show them
                Console.WriteLine("Algorithm '" + entry + "' solved {0} sudokus (cleaned {1}).", solveCount, cleanCount);
                Console.WriteLine("Cleaning:\ttotal: {0}ms\tmean: {1}ms", cleanTimeSum, ((double)cleanTimeSum) / cleanCount);
                Console.WriteLine("Solving:\ttotal: {0}ms\tmean: {1}ms", solveTimeSum, ((double)solveTimeSum) / solveCount);
                Console.WriteLine("Total:\t\ttotal: {0}ms\tmean: {1}ms", wholeTimeSum, ((double)wholeTimeSum) / solveCount);
                Console.WriteLine("Nodes expanded:\ttotal: {0}\tmean: {1}", expandedNodeCountSolved, ((double)expandedNodeCountSolved) / solveCount);
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
