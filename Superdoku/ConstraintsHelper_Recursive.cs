using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class that helps solving a sudoku using constraints.</summary>
    class ConstraintsHelper_Recursive : ConstraintsHelper
    {
        /// <summary>The strategies that can be applied.</summary>
        static private SudokuConstraintsStrategy[] strategies = new SudokuConstraintsStrategy[]
        {
            new SudokuConstraintsStrategy_OneValueLeft(),
            new SudokuConstraintsStrategy_ValueOnceInUnit(),
            new SudokuConstraintsStrategy_NakedTwins()
        };

        /// <summary>The index of the 'one value left' strategy.</summary>
        public const int STRATEGY_ONE_VALUE_LEFT = 0;
        /// <summary>The index of the 'value once in unit' strategy.</summary>
        public const int STRATEGY_VALUE_ONCE_IN_UNIT = 1;
        /// <summary>The index of the 'naked twins' strategy.</summary>
        public const int STRATEGY_NAKED_TWINS = 2;

        /// <summary>Whether or not certain strategies should be applied.</summary>
        private bool[] applyStrategies;

        /// <summary>Constructor.</summary>
        /// <param name="sudoku">The sudoku we will be manipulating.</param>
        public ConstraintsHelper_Recursive(Sudoku sudoku)
            : base(sudoku)
        {
            applyStrategies = new bool[strategies.Length];
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

        /// <summary>Eliminates the given value from the possibilities of the given square. While eliminating this value several strategies are applied to (partially) solve the sudoku.</summary>
        /// <param name="index">The index of the square we want to eliminate the value from.</param>
        /// <param name="value">The value we want to eliminate.</param>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public bool eliminate(int index, int value)
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
            for(int i = 0; i < strategies.Length; ++i)
            {
                if(applyStrategies[i])
                {
                    if(!strategies[i].apply(this, index, value))
                        return false;
                }
            }

            // If we have come here, everything must have gone the right way
            return true;
        }

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

    /// <summary>A factor for the ConstraintsHelper_Recursive class.</summary>
    class ConstraintsHelperFactory_Recursive : ConstraintsHelperFactory
    {
        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelper_Recursive(sudoku); }
    }

    /// <summary>Class that represents a strategy that can be applied after removing a possibility from a sqaure.</summary>
    abstract class SudokuConstraintsStrategy
    {
        /// <summary>Applies the strategy to the given sudoku.</summary>
        /// <param name="sudoku">The SudokuConstraintsHelper that calls this strategy.</param>
        /// <param name="index">The index of the square from which a possibility is removed.</param>
        /// <param name="value">The removed possibility.</param>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public abstract bool apply(ConstraintsHelper_Recursive sudokuConstraintsHelper, int index, int value);
    }

    /// <summary>
    /// Implements the following strategy:
    /// If we have only one value left in our square, we can remove that value from all its peers.
    /// </summary>
    class SudokuConstraintsStrategy_OneValueLeft : SudokuConstraintsStrategy
    {
        public override bool apply(ConstraintsHelper_Recursive sudokuConstraintsHelper, int index, int value)
        {
            // We will need to request the peers of the square
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudokuConstraintsHelper.Sudoku.N);

            // We can only apply this strategy if the square has only one possibility left
            if(sudokuConstraintsHelper.Sudoku[index].Count == 1)
            {
                // Eliminate the value from all peers of the square
                int[] peers = sudokuIndexHelper.getPeersFor(index);
                for(int i = 0; i < peers.Length; ++i)
                {
                    if(!sudokuConstraintsHelper.eliminate(peers[i], sudokuConstraintsHelper.Sudoku[index][0]))
                        return false;
                }
            }

            // If we have come here, everything went successfully
            return true;
        }
    }
    
    /// <summary>
    /// Implements the following strategy:
    /// If the value only appears once as a possibility in a unit, then that sqaure must have that value.
    /// </summary>
    class SudokuConstraintsStrategy_ValueOnceInUnit : SudokuConstraintsStrategy
    {
        public override bool apply(ConstraintsHelper_Recursive sudokuConstraintsHelper, int index, int value)
        {
            // We will need to request the units of the square
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudokuConstraintsHelper.Sudoku.N);

            // We loop through all units of this square, counting the amount of times the value occurs
            int[,] units = sudokuIndexHelper.getUnitsFor(index);
            for(int i = 0; i < units.GetLength(0); ++i)
            {
                // Count the amount of times the value occurs
                int occurredAtIndex = 0;
                int count = 0;
                for(int j = 0; j < units.GetLength(1); ++j)
                {
                    if(sudokuConstraintsHelper.Sudoku[units[i, j]].Contains(value))
                    {
                        ++count;
                        occurredAtIndex = units[i, j];
                    }
                }

                // If the value does not occur at all as a possibility, we have reached a contradiction
                if(count == 0)
                    return false;

                // If the value occurs only once, we apply the strategy
                if(count == 1)
                {
                    if(!sudokuConstraintsHelper.assign(occurredAtIndex, value))
                        return false;
                }
            }

            // If we have come here, everything went successfully
            return true;
        }
    }

    /// <summary>
    /// Implements the following strategy:
    /// If two squares in the same unit contain the same two possibilities and do not contain any other possibilities,
    /// then these possibilities can be eliminated from all other squares in the unit.
    /// </summary>
    class SudokuConstraintsStrategy_NakedTwins : SudokuConstraintsStrategy
    {
        public override bool apply(ConstraintsHelper_Recursive sudokuConstraintsHelper, int index, int value)
        {
            // Check if the square we are examining actually contains two possibilities
            if(sudokuConstraintsHelper.Sudoku[index].Count != 2)
                return true;

            // We will need to request the units of the square
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudokuConstraintsHelper.Sudoku.N);

            // We will try to find two such squares for each unit
            int[,] units = sudokuIndexHelper.getUnitsFor(index);
            for(int unit = 0; unit < units.GetLength(0); ++unit)
            {
                // Loop through all squares in the unit
                for(int square = 0; square < units.GetLength(1); ++square)
                {
                    if(units[unit, square] == index)
                        continue;
                    // Check if the current square has two possibilities and if they are the same as the square we are examining
                    else if(sudokuConstraintsHelper.Sudoku[units[unit, square]].Count == 2 &&
                            sudokuConstraintsHelper.Sudoku[units[unit, square]].Contains(sudokuConstraintsHelper.Sudoku[index][0]) &&
                            sudokuConstraintsHelper.Sudoku[units[unit, square]].Contains(sudokuConstraintsHelper.Sudoku[index][1]))
                    {
                        // Remove the two possibilities from all other squares in the unit
                        int value1 = sudokuConstraintsHelper.Sudoku[index][0];              // Note, we need to remember the values here because they might be removed from our square as
                        int value2 = sudokuConstraintsHelper.Sudoku[index][1];              // a consequence of applying other strategies while eliminating values from other squares
                        for(int i = 0; i < units.GetLength(1); ++i)
                        {
                            if(units[unit, i] != index && units[unit, i] != units[unit, square])
                            {
                                if(!sudokuConstraintsHelper.eliminate(units[unit, i], value1) || !sudokuConstraintsHelper.eliminate(units[unit, i], value2))
                                    return false;
                            }
                        }
                    }
                }
            }

            // If we have come here, everything must have gone successful
            return true;
        }
    }
}
