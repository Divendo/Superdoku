using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class TabuCGAHybrid : LocalSearcher
    {
        /// <summary>Maximum amount of iterations without improvement that cga should run.</summary>
        private int CGAIterationsWithoutImprovement;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement for the tabu searcher (negative value for unlimited).</param>
        /// <param name="CGAIterationsWithoutImprovement">Maximum amount of iterations without improvement that cga should run.</param>
        public TabuCGAHybrid(int maxIterations = -1, int maxIterationsWithoutImprovement = -1, int CGAIterationsWithoutImprovement = 10)
            : base(maxIterations, maxIterationsWithoutImprovement)
        {
            this.CGAIterationsWithoutImprovement = CGAIterationsWithoutImprovement;
        }

        public override bool solve(LocalSudoku sudoku)
        {
            // First we try to find a solution using CGA
            CulturalGeneticAlgorithm cga = new CulturalGeneticAlgorithm_Tournament(maxIterations, CGAIterationsWithoutImprovement);
            bool solved = cga.solve(sudoku);
            bestSolution = cga.Solution;
            iterations = cga.Iterations;
            if(solved)
                return true;
           
            // If that did not work, we try simulated annealing
            TabuSearcher sa = new TabuSearcher(maxIterations - iterations, maxIterationsWithoutImprovement);
            solved = sa.solve(bestSolution);
            bestSolution = sa.Solution;
            iterations += cga.Iterations;
            return solved;
        }
    }
}
