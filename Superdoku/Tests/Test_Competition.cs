using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class Test_Competition : Test
    {
        public override void runTest(Sudoku[] sudokus, string filename = null)
        {
            // We will only test the first sudoku
            Sudoku sudoku = sudokus[0];

            // The constraint propagation strategies
            Dictionary<string, ConstraintsHelperFactory> constraintFactories = new Dictionary<string, ConstraintsHelperFactory>();
            constraintFactories.Add("Depth first - AC1", new ConstraintsHelperFactory_AC1());
            constraintFactories.Add("Depth first - AC3", new ConstraintsHelperFactory_AC3());
            constraintFactories.Add("Depth first - AC3 squares", new ConstraintsHelperFactory_AC3_squares());
            constraintFactories.Add("Depth first - Recursive", new ConstraintsHelperFactory_Recursive());
            constraintFactories.Add("Depth first - AC3 inspired", new ConstraintsHelperFactory_MAC());

            // The local search algorithms
            Dictionary<string, LocalSearcher> localSearchers = new Dictionary<string, LocalSearcher>();
            localSearchers.Add("CGA Roulette", new CulturalGeneticAlgorithm_Roulette(10000, -1, 40));
            localSearchers.Add("CGA Tournament", new CulturalGeneticAlgorithm_Tournament(50000, -1, 40));
            localSearchers.Add("Random restart", new RandomRestartSearcher(-1, 500));
            localSearchers.Add("Simulated annealing", new SimulatedAnnealer(1000000, -1, 16.0, new SimulatedAnnealingCoolingScheme_Exponential(0.9)));
            localSearchers.Add("Simulated annealing CGA hybrid", new SimulatedAnnealingCGAHybrid(1000000, -1, 20));
            localSearchers.Add("Tabu", new TabuSearcher(10000, -1, 9));
            localSearchers.Add("Tabu CGA hybrid", new TabuCGAHybrid(10000, -1, 10));
            localSearchers.Add("GLS", new GeneticLocalSearcher(new IterativeSearcher(-1, 10), 10000, -1));
            localSearchers.Add("ILS", new IteratedLocalSearch(new IterativeSearcher(-1, 10), 10000, -1));

            // Things we are going to measure
            Dictionary<string, bool> solved = new Dictionary<string, bool>();
            Dictionary<string, long> solveTime = new Dictionary<string, long>();

            // We will want to measure the performance of each depth-first search strategy
            foreach(KeyValuePair<string, ConstraintsHelperFactory> entry in constraintFactories)
            {
                // Initialise the necessary objects
                DepthFirstSearch depthFirstSearch = new DepthFirstSearch(entry.Value);
                Sudoku copy = new Sudoku(sudoku);
                Stopwatch stopWatch = new Stopwatch();

                // Initially we have not solved the sudoku
                solved.Add(entry.Key, false);

                // Measure the time it takes to clean the sudoku
                long time = 0;
                stopWatch.Start();
                long cleanResult = depthFirstSearch.clean(copy);
                stopWatch.Stop();
                time = stopWatch.ElapsedTicks;
                stopWatch.Reset();

                // Stop if the sudoku could not be cleaned
                if(cleanResult == -1)
                {
                    Console.WriteLine("Algorithm '" + entry.Key + "' failed to clean the sudoku.");
                    continue;
                }

                // Measure the time it takes to solve the sudoku
                try
                {
                    stopWatch.Start();
                    Sudoku solvedSudoku = depthFirstSearch.search(copy);
                    stopWatch.Stop();
                    time += stopWatch.ElapsedTicks;
                    stopWatch.Reset();

                    // Record the time measurement
                    solveTime.Add(entry.Key, time);

                    // Show the result
                    if(solvedSudoku == null)
                        Console.WriteLine("Algorithm '" + entry.Key + "' failed to solve the sudoku.");
                    else if(!solvedSudoku.isSolved())
                        Console.WriteLine("Algorithm '" + entry.Key + "' appeared to have solved the sudoku, but failed.");
                    else
                    {
                        solved[entry.Key] = true;
                        Console.WriteLine("Algorithm '" + entry.Key + "' solved the sudoku.");
                    }
                }
                catch(Exception exc)
                {
                    Console.WriteLine("Algorithm '" + entry.Key + "' threw exception: " + exc.Message);
                }
            }

            // Test each local search algorithm
            foreach(KeyValuePair<string, LocalSearcher> entry in localSearchers)
            {
                // Initialise the necessary objects
                LocalSearcher localSearcher = entry.Value;
                Stopwatch stopWatch = new Stopwatch();

                // Initially we have not solved the sudoku
                solved.Add(entry.Key, false);

                // Measure the time it takes to solve the sudoku
                try
                {
                    stopWatch.Start();
                    localSearcher.solve(sudoku);
                    stopWatch.Stop();
                    long searchTime = stopWatch.ElapsedTicks;
                    stopWatch.Reset();

                    // Record the measurements
                    solved[entry.Key] = localSearcher.Solution.toSudoku().isSolved();
                    solveTime.Add(entry.Key, searchTime);

                    // Show that this algorithm is done
                    Console.WriteLine("Algorithm '" + entry.Key + "' done.");
                }
                catch(Exception exc)
                {
                    Console.WriteLine("Algorithm '" + entry.Key + "' threw exception: " + exc.Message);
                }
            }

            // Create an exporter
            ResultsExporter resultsExporter = null;
            if(filename != null)
            {
                resultsExporter = new ResultsExporter(filename);
                resultsExporter.addExtraValue("Ticks per second", Stopwatch.Frequency);
            }

            // Calculate and show the stats
            Console.WriteLine("Results (from {0} sudokus):", sudokus.Length);
            Console.WriteLine("--------------------");
            foreach(string entry in solved.Keys)
            {
                // Add the results to the exporter
                if(resultsExporter != null)
                {
                    resultsExporter.addResult(entry, "solved", solved[entry] ? 1 : 0);
                    resultsExporter.addResult(entry, "solve time", solved[entry] ? solveTime[entry] : -1);
                }

                // Then we show them
                if(solved[entry])
                    Console.WriteLine("Algorithm '{0}' solved the sudoku in {1}ms.", entry, ((double)solveTime[entry]) / Stopwatch.Frequency);
                else
                    Console.WriteLine("Algorithm '{0}' was unable to solve the sudoku.", entry);
            }
            Console.WriteLine("--------------------");

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
