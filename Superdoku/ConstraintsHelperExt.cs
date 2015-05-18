using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    abstract class ConstraintsHelperExt : ConstraintsHelper
    {
        /// <summary>The strategies that can be applied.</summary>
        static protected ConstraintsHelperExt_StrategyFactory[] strategyFactories = new ConstraintsHelperExt_StrategyFactory[]
        {
            new ConstraintsHelperExt_StrategyFactory_OneValueLeft(),
            new ConstraintsHelperExt_StrategyFactory_ValueOnceInUnit(),
            new ConstraintsHelperExt_StrategyFactory_NakedTwins()
        };

        /// <summary>The index of the 'one value left' strategy.</summary>
        public const int STRATEGY_ONE_VALUE_LEFT = 0;
        /// <summary>The index of the 'value once in unit' strategy.</summary>
        public const int STRATEGY_VALUE_ONCE_IN_UNIT = 1;
        /// <summary>The index of the 'naked twins' strategy.</summary>
        public const int STRATEGY_NAKED_TWINS = 2;

        /// <summary>Whether or not certain strategies should be applied.</summary>
        protected bool[] applyStrategies;

        /// <summary>Constructor.</summary>
        /// <param name="sudoku">The sudoku we will be manipulating.</param>
        public ConstraintsHelperExt(Sudoku sudoku)
            : base(sudoku)
        {
            applyStrategies = new bool[strategyFactories.Length];
            for(int i = 0; i < applyStrategies.Length; ++i)
                applyStrategies[i] = true;
        }

        /// <summary>Sets whether or not a certain strategy should be used.</summary>
        /// <param name="strategy">The strategy (use the STRATEGY_* constants).</param>
        /// <param name="use">Whether or not the strategy should be used.</param>
        public void setStrategyUse(int strategy, bool use)
        { applyStrategies[strategy] = use; }

        /// <summary>Returns whether or not a certain strategy is being used.</summary>
        /// <param name="strategy">The strategy (use the STRATEGY_* constants).</param>
        /// <returns>True if the strategy is being used, false otherwise.</returns>
        public bool isStrategyUsed(int strategy)
        { return applyStrategies[strategy]; }

        /// <summary>Eliminates the given value from the possibilities of the given square. While eliminating this value several strategies are applied to (partially) solve the sudoku.</summary>
        /// <param name="index">The index of the square we want to eliminate the value from.</param>
        /// <param name="value">The value we want to eliminate.</param>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public abstract bool eliminate(int index, int value);

        public override bool clean()
        {
            // A list of all values
            List<int> allValues = new List<int>(sudoku.NN);
            for(int i = 0; i < sudoku.NN; ++i)
                allValues.Add(i);

            // First we build a list of indices that we want to apply assign() to
            // Also keep track of there values because we are going to fill the squares with all possibilities again
            List<int> indices = new List<int>();
            List<int> values = new List<int>();
            for(int i = 0; i < sudoku.NN * sudoku.NN; ++i)
            {
                if(sudoku[i].Count == 1)
                {
                    indices.Add(i);
                    values.Add(sudoku[i][0]);
                    sudoku[i] = new List<int>(allValues);
                }
            }

            // Now we apply assign to each of these indices
            for(int i = 0; i < indices.Count; ++i)
            {
                if(!assign(indices[i], values[i]))
                    return false;
            }

            // If we have come here, everything went successful
            return true;
        }
    }
}
