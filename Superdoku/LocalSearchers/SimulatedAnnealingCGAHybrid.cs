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
        private const int CGA_ITERATIONS_WITHOUT_IMPROVEMENT = 10;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public SimulatedAnnealingCGAHybrid(int maxIterations = -1)
            : base(maxIterations) { }

        public override bool solve(LocalSudoku sudoku)
        {
            // First we try to find a solution using CGA
            CulturalGeneticAlgorithm cga = new CulturalGeneticAlgorithm_Tournament(maxIterations, CGA_ITERATIONS_WITHOUT_IMPROVEMENT);
            bool solved = cga.solve(sudoku);
            bestSolution = cga.Solution;
            iterations = cga.Iterations;
            if(solved)
                return true;
           
            // If that did not work, we try simulated annealing
            SimulatedAnnealer sa = new SimulatedAnnealer(maxIterations - iterations);
            solved = sa.solve(bestSolution);
            bestSolution = sa.Solution;
            iterations += cga.Iterations;
            return solved;
        }
    }
}
