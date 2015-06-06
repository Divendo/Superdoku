﻿using System;
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
            bool testDepthFirstSearchAlgorithm = false;

            // Read and test the sudokus
            if(testMultipleSudokus)
            {
                // Read the sudokus
                Sudoku[] sudokus = SudokuReader.readFromFileLines("../../sudokus/gordon-royle-49151-9x9.txt", 3, 5);
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
            // Create the search machine
            LocalSearcher searchMachine = new GeneticLocalSearcher(new IterativeSearcher(1000, 10));

            // Measure the performance
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Sudoku solution = searchMachine.solve(sudoku);
            stopWatch.Stop();

            // Show the result
            Console.WriteLine("The local searcher finished in {0} ms.", stopWatch.ElapsedMilliseconds);
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
            // The default maximum iterations and maximum iterations without improvement
            int defaultMaxIterations = 50000;
            int defaultMaxIterationsWithoutImprovement = 800;

            // We will solve the sudoku using different local search techniques
            Dictionary<string, LocalSearcher> constraintFactories = new Dictionary<string, LocalSearcher>();
            constraintFactories.Add("CGA Roulette", new CulturalGeneticAlgorithm_Roulette(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("CGA Tournament", new CulturalGeneticAlgorithm_Tournament(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Iterative", new IterativeSearcher(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Random restart", new RandomRestartSearcher(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Randwom walk", new RandomWalkSearcher(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Simulated annealing", new SimulatedAnnealer(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Simulated annealing CGA hybrid", new SimulatedAnnealingCGAHybrid(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Tabu", new TabuSearcher(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Tabu CGA hybrid", new TabuCGAHybrid(defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Genetic Local Search ITERATIVE", new GeneticLocalSearcher(new IterativeSearcher(defaultMaxIterations, 10) , defaultMaxIterations, defaultMaxIterationsWithoutImprovement));
            constraintFactories.Add("Genetic Local Search Tabu", new GeneticLocalSearcher(new TabuSearcher(defaultMaxIterations, 5), defaultMaxIterations, defaultMaxIterationsWithoutImprovement));

            // Things we are going to measure
            Dictionary<string, long[]> solveTimes = new Dictionary<string, long[]>();
            Dictionary<string, bool[]> solved = new Dictionary<string, bool[]>();
            Dictionary<string, int[]> heuristicValues = new Dictionary<string, int[]>();
            Dictionary<string, long[]> iterations = new Dictionary<string, long[]>();

            // Initialise the measure dictionaries
            foreach(KeyValuePair<string, LocalSearcher> entry in constraintFactories)
            {
                solveTimes.Add(entry.Key, new long[sudokus.Length]);
                solved.Add(entry.Key, new bool[sudokus.Length]);
                heuristicValues.Add(entry.Key, new int[sudokus.Length]);
                iterations.Add(entry.Key, new long[sudokus.Length]);
            }

            // Measure the performance on each sudoku
            for(int i = 0; i < sudokus.Length; ++i)
            {
                // Show which sudoku we are solving
                Console.WriteLine("Solving sudoku " + i.ToString());

                // We will want to measure the performance of each strategy
                foreach(KeyValuePair<string, LocalSearcher> entry in constraintFactories)
                {
                    // Initialise the necessary objects
                    LocalSearcher localSearcher = entry.Value;
                    Sudoku copy = new Sudoku(sudokus[i]);
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
            ResultsExporter resultsExporter = new ResultsExporter("local-search.csv");
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
                for(int i = 0; i < sudokus.Length; ++i)
                {
                    heuristicSum += heuristicValues[entry][i];
                    iterationSum += iterations[entry][i];

                    if(solved[entry][i])
                    {
                        ++solveCount;
                        solveTimeSum += solveTimes[entry][i];
                        iterationSumSolved += iterations[entry][i];
                    }
                }

                // Add the results to the exporter
                resultsExporter.addResult(entry, "solve time", solveTimeSum);
                resultsExporter.addResult(entry, "solve count", solveCount);
                resultsExporter.addResult(entry, "heuristic value", heuristicSum);
                resultsExporter.addResult(entry, "iteration count", iterationSum);
                resultsExporter.addResult(entry, "iteration count (solved)", iterationSumSolved);
                resultsExporter.addResult(entry, "sudoku count", sudokus.Length);

                // Then we show them
                Console.WriteLine("Algorithm '" + entry + "' solved {0} sudokus.", solveCount);
                Console.WriteLine("Solving:\t\ttotal: {0}ms\tmean: {1}ms", solveTimeSum, ((double)solveTimeSum) / solveCount);
                Console.WriteLine("Heuristic:\t\ttotal: {0}\tmean: {1}", heuristicSum, ((double)heuristicSum) / sudokus.Length);
                Console.WriteLine("Iterations:\t\ttotal: {0}\tmean: {1}", iterationSum, ((double)iterationSum) / sudokus.Length);
                Console.WriteLine("Iterations (solved):\ttotal: {0}\tmean: {1}", iterationSumSolved, ((double)iterationSumSolved) / solveCount);
                Console.WriteLine("--------------------");
            }
            resultsExporter.write();
            Console.WriteLine("Results have been exported to '{0}'.", resultsExporter.Filename);
        }

        static void testDepthFirstSearch(Sudoku sudoku)
        {
            // We will solve the sudoku using depth-first search and different constraint strategies
            Dictionary<string, ConstraintsHelperFactory> constraintFactories = new Dictionary<string, ConstraintsHelperFactory>();
            constraintFactories.Add("AC1", new ConstraintsHelperFactory_AC1());
            constraintFactories.Add("AC3", new ConstraintsHelperFactory_AC3());
            constraintFactories.Add("AC3 squares", new ConstraintsHelperFactory_AC3_squares());
            constraintFactories.Add("recursive", new ConstraintsHelperFactory_Recursive());
            constraintFactories.Add("MAC", new ConstraintsHelperFactory_MAC());
            //constraintFactories.Add("trivial", new ConstraintsHelperFactory_Trivial());

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

            // Calculate and show the stats
            ResultsExporter resultsExporter = new ResultsExporter("depth-first-search.csv");
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
                resultsExporter.addResult(entry, "clean time", cleanTimeSum);
                resultsExporter.addResult(entry, "clean count", cleanCount);
                resultsExporter.addResult(entry, "solve time", solveTimeSum);
                resultsExporter.addResult(entry, "solve count", solveCount);
                resultsExporter.addResult(entry, "total time", wholeTimeSum);
                resultsExporter.addResult(entry, "expanded nodes", expandedNodeCount);
                resultsExporter.addResult(entry, "expanded nodes (solved)", expandedNodeCountSolved);
                resultsExporter.addResult(entry, "sudoku count", sudokus.Length);

                // Then we show them
                Console.WriteLine("Algorithm '" + entry + "' solved {0} sudokus (cleaned {1}).", solveCount, cleanCount);
                Console.WriteLine("Cleaning:\ttotal: {0}ms\tmean: {1}ms", cleanTimeSum, ((double)cleanTimeSum) / cleanCount);
                Console.WriteLine("Solving:\ttotal: {0}ms\tmean: {1}ms", solveTimeSum, ((double)solveTimeSum) / solveCount);
                Console.WriteLine("Total:\t\ttotal: {0}ms\tmean: {1}ms", wholeTimeSum, ((double)wholeTimeSum) / solveCount);
                Console.WriteLine("Nodes expanded:\ttotal: {0}\tmean: {1}", expandedNodeCountSolved, ((double)expandedNodeCountSolved) / solveCount);
                Console.WriteLine("--------------------");
            }
            resultsExporter.write();
            Console.WriteLine("Results have been exported to '{0}'.", resultsExporter.Filename);
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
                    if(sudoku[x, y] == 0)
                        Console.Write('x');
                    else
                    {
                        int singleValue = sudoku.singleValue(x, y);
                        if(singleValue != -1)
                        {
                            if(singleValue <= 9)
                                Console.Write(singleValue);
                            else
                                Console.Write((char)('A' + (singleValue - 10)));
                        }
                        else
                            Console.Write('.');
                    }
                }

                // Next row
                Console.Write('\n');
            }
        }
    }
}
