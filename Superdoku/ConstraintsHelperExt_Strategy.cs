using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class that represents a strategy that can be applied after removing a possibility from a sqaure.</summary>
    abstract class ConstraintsHelperExt_Strategy
    {
        /// <summary>The ConstraintsHelperExt that calls this strategy.</summary>
        protected ConstraintsHelperExt constraintsHelper;
        /// <summary>The index of the square from which a possibility is removed.</summary>
        protected int square;
        /// <summary>The removed possibility.</summary>
        protected int removedValue;

        /// <summary>Constructor.</summary>
        /// <param name="sudoku">The ConstraintsHelper_MAC that calls this strategy.</param>
        /// <param name="square">The index of the square from which a possibility is removed.</param>
        /// <param name="removedValue">The removed possibility.</param>
        public ConstraintsHelperExt_Strategy(ConstraintsHelperExt constraintsHelper, int square, int removedValue)
        {
            this.constraintsHelper = constraintsHelper;
            this.square = square;
            this.removedValue = removedValue;
        }

        /// <summary>Applies the strategy to the given sudoku.</summary>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public abstract bool apply();
    }

    /// <summary>Factory for the ConstraintsHelperExt_Strategy class.</summary>
    abstract class ConstraintsHelperExt_StrategyFactory
    {
        /// <summary>Constructs a new strategy.</summary>
        /// <param name="sudoku">The ConstraintsHelperExt that calls this strategy.</param>
        /// <param name="square">The index of the square from which a possibility is removed.</param>
        /// <param name="removedValue">The removed possibility.</param>
        /// <returns>The created strategy.</returns>
        public abstract ConstraintsHelperExt_Strategy createStrategy(ConstraintsHelperExt constraintsHelper, int square, int removedValue);
    }

    /// <summary>
    /// Implements the following strategy:
    /// If we have only one value left in our square, we can remove that value from all its peers.
    /// </summary>
    class ConstraintsHelperExt_Strategy_OneValueLeft : ConstraintsHelperExt_Strategy
    {
        public ConstraintsHelperExt_Strategy_OneValueLeft(ConstraintsHelperExt constraintsHelper, int square, int removedValue)
            : base(constraintsHelper, square, removedValue) { }

        public override bool Equals(object obj)
        {
            if(!(obj is ConstraintsHelperExt_Strategy_OneValueLeft))
                return false;

            // The removed value is not used in this class, and is thus ignored
            ConstraintsHelperExt_Strategy_OneValueLeft other = (ConstraintsHelperExt_Strategy_OneValueLeft)obj;
            return square == other.square;
        }

        public override int GetHashCode()
        {
            // The removed value is not used in this class, and is thus ignored
            return square * constraintsHelper.Sudoku.NN * 10 + 1;
        }

        public override bool apply()
        {
            // We will need to request the peers of the square
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(constraintsHelper.Sudoku.N);

            // We can only apply this strategy if the square has only one possibility left
            if(constraintsHelper.Sudoku[square].Count == 1)
            {
                // Eliminate the value from all peers of the square
                int[] peers = sudokuIndexHelper.getPeersFor(square);
                for(int i = 0; i < peers.Length; ++i)
                {
                    if(!constraintsHelper.eliminate(peers[i], constraintsHelper.Sudoku[square][0]))
                        return false;
                }
            }

            // If we have come here, everything went successfully
            return true;
        }
    }

    /// <summary>Factory class for the ConstraintsHelperExt_Strategy_OneValueLeft class.</summary>
    class ConstraintsHelperExt_StrategyFactory_OneValueLeft : ConstraintsHelperExt_StrategyFactory
    {
        public override ConstraintsHelperExt_Strategy createStrategy(ConstraintsHelperExt constraintsHelper, int square, int removedValue)
        {
            return new ConstraintsHelperExt_Strategy_OneValueLeft(constraintsHelper, square, removedValue);
        }
    }

    /// <summary>
    /// Implements the following strategy:
    /// If the value only appears once as a possibility in a unit, then that sqaure must have that value.
    /// </summary>
    class ConstraintsHelperExt_Strategy_ValueOnceInUnit : ConstraintsHelperExt_Strategy
    {
        public ConstraintsHelperExt_Strategy_ValueOnceInUnit(ConstraintsHelperExt constraintsHelper, int square, int removedValue)
            : base(constraintsHelper, square, removedValue) { }

        public override bool Equals(object obj)
        {
            if(!(obj is ConstraintsHelperExt_Strategy_ValueOnceInUnit))
                return false;

            ConstraintsHelperExt_Strategy_ValueOnceInUnit other = (ConstraintsHelperExt_Strategy_ValueOnceInUnit)obj;
            return square == other.square && removedValue == other.removedValue;
        }

        public override int GetHashCode()
        {
            return (square * constraintsHelper.Sudoku.NN * constraintsHelper.Sudoku.NN + removedValue * constraintsHelper.Sudoku.NN) * 10 + 2;
        }

        public override bool apply()
        {
            // We will need to request the units of the square
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(constraintsHelper.Sudoku.N);

            // We loop through all units of this square, counting the amount of times the value occurs
            int[,] units = sudokuIndexHelper.getUnitsFor(square);
            for(int i = 0; i < units.GetLength(0); ++i)
            {
                // Count the amount of times the value occurs
                int occurredAtIndex = 0;
                int count = 0;
                for(int j = 0; j < units.GetLength(1); ++j)
                {
                    if(constraintsHelper.Sudoku[units[i, j]].Contains(removedValue))
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
                    if(!constraintsHelper.assign(occurredAtIndex, removedValue))
                        return false;
                }
            }

            // If we have come here, everything went successfully
            return true;
        }
    }

    /// <summary>Factory class for the ConstraintsHelperExt_Strategy_ValueOnceInUnit class.</summary>
    class ConstraintsHelperExt_StrategyFactory_ValueOnceInUnit : ConstraintsHelperExt_StrategyFactory
    {
        public override ConstraintsHelperExt_Strategy createStrategy(ConstraintsHelperExt constraintsHelper, int square, int removedValue)
        {
            return new ConstraintsHelperExt_Strategy_ValueOnceInUnit(constraintsHelper, square, removedValue);
        }
    }

    /// <summary>
    /// Implements the following strategy:
    /// If two squares in the same unit contain the same two possibilities and do not contain any other possibilities,
    /// then these possibilities can be eliminated from all other squares in the unit.
    /// </summary>
    class ConstraintsHelperExt_Strategy_NakedTwins : ConstraintsHelperExt_Strategy
    {
        public ConstraintsHelperExt_Strategy_NakedTwins(ConstraintsHelperExt constraintsHelper, int square, int removedValue)
            : base(constraintsHelper, square, removedValue) { }

        public override bool Equals(object obj)
        {
            if(!(obj is ConstraintsHelperExt_Strategy_NakedTwins))
                return false;

            // The removed value is not used in this class, and is thus ignored
            ConstraintsHelperExt_Strategy_NakedTwins other = (ConstraintsHelperExt_Strategy_NakedTwins)obj;
            return square == other.square;
        }

        public override int GetHashCode()
        {
            // The removed value is not used in this class, and is thus ignored
            return square * constraintsHelper.Sudoku.NN * 10 + 3;
        }

        public override bool apply()
        {
            // Check if the square we are examining actually contains two possibilities
            if(constraintsHelper.Sudoku[square].Count != 2)
                return true;

            // We will need to request the units of the square
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(constraintsHelper.Sudoku.N);

            // We will try to find two such squares for each unit
            int[,] units = sudokuIndexHelper.getUnitsFor(square);
            for(int unit = 0; unit < units.GetLength(0); ++unit)
            {
                // Loop through all squares in the unit
                for(int squareInUnit = 0; squareInUnit < units.GetLength(1); ++squareInUnit)
                {
                    if(units[unit, squareInUnit] == square)
                        continue;
                    // Check if the current square has two possibilities and if they are the same as the square we are examining
                    else if(constraintsHelper.Sudoku[units[unit, squareInUnit]].Count == 2 &&
                            constraintsHelper.Sudoku[units[unit, squareInUnit]].Contains(constraintsHelper.Sudoku[square][0]) &&
                            constraintsHelper.Sudoku[units[unit, squareInUnit]].Contains(constraintsHelper.Sudoku[square][1]))
                    {
                        // Remove the two possibilities from all other squares in the unit
                        int value1 = constraintsHelper.Sudoku[square][0];              // Note, we need to remember the values here because they might be removed from our square as
                        int value2 = constraintsHelper.Sudoku[square][1];              // a consequence of applying other strategies while eliminating values from other squares
                        for(int i = 0; i < units.GetLength(1); ++i)
                        {
                            if(units[unit, i] != square && units[unit, i] != units[unit, squareInUnit])
                            {
                                if(!constraintsHelper.eliminate(units[unit, i], value1) || !constraintsHelper.eliminate(units[unit, i], value2))
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

    /// <summary>Factory class for the ConstraintsHelperExt_Strategy_NakedTwins class.</summary>
    class ConstraintsHelperExt_StrategyFactory_NakedTwins : ConstraintsHelperExt_StrategyFactory
    {
        public override ConstraintsHelperExt_Strategy createStrategy(ConstraintsHelperExt constraintsHelper, int square, int removedValue)
        {
            return new ConstraintsHelperExt_Strategy_NakedTwins(constraintsHelper, square, removedValue);
        }
    }
}
