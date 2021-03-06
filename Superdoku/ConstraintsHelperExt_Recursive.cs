﻿using System;
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
        /// <param name="strategies">Whether or not certain strategies should be applied.</param>
        public ConstraintsHelperExt_Recursive(Sudoku sudoku, bool[] strategies = null)
            : base(sudoku, strategies) { }

        public override bool assign(int index, ulong value)
        {
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

            // If we have come here, everything must have gone the right way
            return true;
        }

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

            // Apply the strategies
            for(int i = 0; i < strategyFactories.Length; ++i)
            {
                if(applyStrategies[i])
                {
                    ++iterations;
                    ConstraintsHelperExt_Strategy strategy = strategyFactories[i].createStrategy(this, index, value);
                    if(!strategy.apply())
                        return false;
                }
            }

            // If we have come here, everything must have gone the right way
            return true;
        }
    }

    /// <summary>A factory for the ConstraintsHelper_Recursive class.</summary>
    class ConstraintsHelperFactory_Recursive : ConstraintsHelperFactory_Ext
    {
        /// <summary>Constructor.</summary>
        /// <param name="strategies">Whether or not certain strategies should be applied.</param>
        public ConstraintsHelperFactory_Recursive(bool[] strategies = null)
            : base(strategies) { }

        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelperExt_Recursive(sudoku, applyStrategies); }
    }
}
