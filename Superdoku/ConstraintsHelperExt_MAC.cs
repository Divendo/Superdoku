using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class that helps solving a sudoku using constraints. The strategies will be applied one after each other using a queue.</summary>
    class ConstraintsHelperExt_MAC : ConstraintsHelperExt
    {
        /// <summary>The strategies that still should be run.</summary>
        private HashSet<ConstraintsHelperExt_Strategy> strategiesToRun;

        /// <summary>The strategies that still should be run (in queue form).</summary>
        private Queue<ConstraintsHelperExt_Strategy> strategiesQueue;

        /// <summary>Constructor.</summary>
        /// <param name="sudoku">The sudoku we will be manipulating.</param>
        /// <param name="strategies">Whether or not certain strategies should be applied.</param>
        public ConstraintsHelperExt_MAC(Sudoku sudoku, bool[] strategies = null)
            : base(sudoku, strategies) { }

        public override bool assign(int index, ulong value)
        {
            // Initialise the strategy queue and set
            strategiesToRun = new HashSet<ConstraintsHelperExt_Strategy>();
            strategiesQueue = new Queue<ConstraintsHelperExt_Strategy>();

            // First we get the values we want to eliminate
            ulong toEliminate = sudoku[index] ^ value;

            // Eliminate all values
            for(ulong eliminateMe = 1; toEliminate != 0; eliminateMe <<= 1)
            {
                if((toEliminate & eliminateMe) == 0)
                    continue;

                if(!eliminate(index, eliminateMe))
                    return false;

                toEliminate ^= eliminateMe;
            }

            // Keep running strategies while there are strategies to run
            while(strategiesQueue.Count != 0)
            {
                ++iterations;
                ConstraintsHelperExt_Strategy strategy = strategiesQueue.Dequeue();
                strategiesToRun.Remove(strategy);
                if(!strategy.apply())
                    return false;
            }

            // If we have come here, everything must have gone the right way
            return true;
        }

        /// <summary>Eliminates the given value from the possibilities of the given square. While eliminating this value several strategies are applied to (partially) solve the sudoku.</summary>
        /// <param name="index">The index of the square we want to eliminate the value from.</param>
        /// <param name="value">The value we want to eliminate.</param>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public override bool eliminate(int index, ulong value)
        {
            // If the value is not present in the given square, we can stop here
            if((sudoku[index] & value) == 0)
                return true;

            // Remove the value
            sudoku[index] ^= value;

            // Check if the sudoku is still correct
            if(sudoku[index] == 0)
                return false;

            // Add strategies
            for(int i = 0; i < strategyFactories.Length; ++i)
            {
                if(applyStrategies[i])
                {
                    ConstraintsHelperExt_Strategy strategy = strategyFactories[i].createStrategy(this, index, value);
                    if(!strategiesToRun.Contains(strategy))
                    {
                        strategiesToRun.Add(strategy);
                        strategiesQueue.Enqueue(strategy);
                    }
                }
            }

            // If we have come here, everything must have gone the right way
            return true;
        }
    }

    /// <summary>A factory for the ConstraintsHelper_Recursive class.</summary>
    class ConstraintsHelperFactory_MAC : ConstraintsHelperFactory_Ext
    {
        /// <summary>Constructor.</summary>
        /// <param name="strategies">Whether or not certain strategies should be applied.</param>
        public ConstraintsHelperFactory_MAC(bool[] strategies = null)
            : base(strategies) { }

        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelperExt_MAC(sudoku, applyStrategies); }
    }
}
