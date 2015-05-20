﻿using System;
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
        public ConstraintsHelperExt_MAC(Sudoku sudoku)
            : base(sudoku) { }

        public override bool assign(int index, int value)
        {
            // Initialise the strategy queue and set
            strategiesToRun = new HashSet<ConstraintsHelperExt_Strategy>();
            strategiesQueue = new Queue<ConstraintsHelperExt_Strategy>();

            // First we get the values we want to eliminate
            List<int> toEliminate = new List<int>(sudoku[index]);
            toEliminate.Remove(value);

            // Eliminate all values
            for(int i = 0; i < toEliminate.Count; ++i)
            {
                if(!eliminate(index, toEliminate[i]))
                    return false;
            }

            // Keep running strategies while there are strategies to run
            while(strategiesQueue.Count != 0)
            {
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
        public override bool eliminate(int index, int value)
        {
            // If the value is not present in the given square, we can stop here
            if(!sudoku[index].Contains(value))
                return true;

            // Remove the value
            sudoku[index].Remove(value);

            // Check if the sudoku is still correct
            if(sudoku[index].Count == 0)
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

    /// <summary>A factor for the ConstraintsHelper_Recursive class.</summary>
    class ConstraintsHelperFactory_MAC : ConstraintsHelperFactory
    {
        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelperExt_MAC(sudoku); }
    }
}