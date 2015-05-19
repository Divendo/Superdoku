using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class that helps solving a sudoku using constraints. The strategies will be applied recursive.</summary>
    class ConstraintsHelperExt_Recursive : ConstraintsHelperExt
    {
        /// <summary>Constructor.</summary>
        /// <param name="sudoku">The sudoku we will be manipulating.</param>
        public ConstraintsHelperExt_Recursive(Sudoku sudoku)
            : base(sudoku) { }

        public override bool assign(int index, int value)
        {
            // First we get the values we want to eliminate
            List<int> toEliminate = new List<int>(sudoku[index]);
            toEliminate.Remove(value);

            // Eliminate all values
            for(int i = 0; i < toEliminate.Count; ++i)
            {
                if(!eliminate(index, toEliminate[i]))
                    return false;
            }

            // If we have come here, everything must have gone the right way
            return true;
        }

        public override bool eliminate(int index, int value)
        {
            // If the value is not present in the given square, we can stop here
            if(!sudoku[index].Contains(value))
                return true;

            // Remove the value
            sudoku[index].Remove(value);

            // Check for a contradiction
            if(sudoku[index].Count == 0)
                return false;

            // Apply the strategies
            for(int i = 0; i < strategyFactories.Length; ++i)
            {
                if(applyStrategies[i])
                {
                    ConstraintsHelperExt_Strategy strategy = strategyFactories[i].createStrategy(this, index, value);
                    if(!strategy.apply())
                        return false;
                }
            }

            // If we have come here, everything must have gone the right way
            return true;
        }
    }

    /// <summary>A factor for the ConstraintsHelper_Recursive class.</summary>
    class ConstraintsHelperFactory_Recursive : ConstraintsHelperFactory
    {
        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelperExt_Recursive(sudoku); }
    }
}
