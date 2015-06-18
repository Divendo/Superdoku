using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class SimulatedAnnealingCGAHybrid : LocalSearcher
    {
        /// <summary>Maximum amount of iterations without improvement that cga should run.</summary>
        private int CGAIterationsWithoutImprovement;

        /// <summary>The amount of iterations spent on running CGA.</summary>
        private int iterationsSpentOnCga;

        /// <summary>The heuristic value after running CGA.</summary>
        private int heuristicValueAfterCga;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement for the simulated annealing searcher (negative value for unlimited).</param>
        /// <param name="CGAIterationsWithoutImprovement">Maximum amount of iterations without improvement that cga should run.</param>
        public SimulatedAnnealingCGAHybrid(int maxIterations = -1, int maxIterationsWithoutImprovement = -1, int CGAIterationsWithoutImprovement = 10)
            : base(maxIterations, maxIterationsWithoutImprovement)
        {
            this.CGAIterationsWithoutImprovement = CGAIterationsWithoutImprovement;
        }

        /// <summary>Property to get the amount of iterations spent on running CGA.</summary>
        public int IterationsSpentOnCga
        { get { return iterationsSpentOnCga; } }

        public int HeuristicValueAfterCga
        { get { return heuristicValueAfterCga; } }

        public override bool solve(LocalSudoku sudoku)
        {
            // First we try to find a solution using CGA
            CulturalGeneticAlgorithm cga = new CulturalGeneticAlgorithm_Tournament(maxIterations, CGAIterationsWithoutImprovement);
            bool solved = cga.solve(sudoku);
            bestSolution = cga.Solution;
            iterations = cga.Iterations;
            iterationsSpentOnCga = cga.Iterations;
            heuristicValueAfterCga = bestSolution.HeuristicValue;
            if(solved)
                return true;
           
            // If that did not work, we try simulated annealing
            SimulatedAnnealer sa = new SimulatedAnnealer(maxIterations - iterations, maxIterationsWithoutImprovement, 16, new SimulatedAnnealingCoolingScheme_Exponential(0.9));
            solved = sa.solve(bestSolution);
            bestSolution = sa.Solution;
            iterations += cga.Iterations;
            return solved;
        }
    }
}
