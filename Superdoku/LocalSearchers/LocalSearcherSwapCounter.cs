using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    abstract class LocalSearcherSwapCounter : LocalSearcher
    {
        /// <summary>The total amount of swaps that were performed.</summary>
        protected int totalSwaps;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        public LocalSearcherSwapCounter(int maxIterations = -1, int maxIterationsWithoutImprovement = -1)
            : base(maxIterations, maxIterationsWithoutImprovement)
        { }

        /// <summary>The total amount of swaps that were performed.</summary>
        public int TotalSwaps
        { get { return totalSwaps; } }
    }
}
