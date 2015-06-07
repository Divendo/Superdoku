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
        protected static ConstraintsHelperExt_StrategyFactory[] strategyFactories = new ConstraintsHelperExt_StrategyFactory[]
        {
            new ConstraintsHelperExt_StrategyFactory_OneValueLeft(),
            new ConstraintsHelperExt_StrategyFactory_ValueOnceInUnit(),
            new ConstraintsHelperExt_StrategyFactory_NakedTwins()
        };

        /// <summary>The amount of strategies there exist.</summary>
        public static int STRATEGY_COUNT = strategyFactories.Length;

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
        /// <param name="strategies">Whether or not certain strategies should be applied.</param>
        public ConstraintsHelperExt(Sudoku sudoku, bool[] strategies = null)
            : base(sudoku)
        {
            applyStrategies = new bool[STRATEGY_COUNT];
            for(int i = 0; i < STRATEGY_COUNT; ++i)
            {
                if(strategies != null && i < strategies.Length)
                    applyStrategies[i] = strategies[i];
                else
                    applyStrategies[i] = true;
            }

            iterations = 0;
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
        public abstract bool eliminate(int index, ulong value);

        public override bool clean()
        {
            // First we build a list of indices that we want to apply assign() to
            // Also keep track of there values because we are going to fill the squares with all possibilities again
            List<int> indices = new List<int>();
            List<ulong> values = new List<ulong>();
            ulong allValues = (1ul << sudoku.NN) - 1;
            for(int i = 0; i < sudoku.NN * sudoku.NN; ++i)
            {
                if(sudoku.valueCount(i) == 1)
                {
                    indices.Add(i);
                    values.Add(sudoku[i]);
                    sudoku[i] = allValues;
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

    /// <summary>An abstract factory for the ConstraintsHelperExt class.</summary>
    abstract class ConstraintsHelperFactory_Ext : ConstraintsHelperFactory
    {
        /// <summary>Whether or not certain strategies should be applied.</summary>
        protected bool[] applyStrategies;
        
        /// <summary>Constructor.</summary>
        /// <param name="strategies">Whether or not certain strategies should be applied.</param>
        public ConstraintsHelperFactory_Ext(bool[] strategies = null)
        {
            applyStrategies = new bool[ConstraintsHelperExt.STRATEGY_COUNT];
            for(int i = 0; i < ConstraintsHelperExt.STRATEGY_COUNT; ++i)
            {
                if(strategies != null && i < strategies.Length)
                    applyStrategies[i] = strategies[i];
                else
                    applyStrategies[i] = true;
            }
        }

        /// <summary>Sets whether or not a certain strategy should be used.</summary>
        /// <param name="strategy">The strategy (use the STRATEGY_* constants).</param>
        /// <param name="use">Whether or not the strategy should be used.</param>
        public void setStrategyUse(int strategy, bool use)
        { applyStrategies[strategy] = use; }
    }
}
