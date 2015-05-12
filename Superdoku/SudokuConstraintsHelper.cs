using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class that helps solving a sudoku using constraints.</summary>
    class SudokuConstraintsHelper
    {
        /// <summary>The sudoku we are manipulating.</summary>
        private Sudoku sudoku;

        /// <summary>Constructor.</summary>
        /// <param name="sudoku">The sudoku we will be manipulating.</param>
        public SudokuConstraintsHelper(Sudoku sudoku)
        { this.sudoku = sudoku; }

        /// <summary>Property to access the sudoku we are manipulating.</summary>
        public Sudoku Sudoku
        {
            get { return sudoku; }
            set { sudoku = value; }
        }

        /// <summary>Assign a value to a square, and apply some strategies to (partially) solve the sudoku.</summary>
        /// <param name="index">The index of the square we want to change.</param>
        /// <param name="value">The value the square should get.</param>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public bool assign(int index, int value)
        {
            // First we get the values we want to eliminate
            List<int> toEliminate = new List<int>(sudoku[index]);
            toEliminate.Remove(value);

            // Eliminate all values
            for(int i = 0; i < toEliminate.Count; ++i)
            {
                if(!eleminate(index, toEliminate[i]))
                    return false;
            }

            // If we have come here, everything must have gone the right way
            return true;
        }

        /// <summary>Eliminates the given value from the possibilities of the given square. While eliminating this value several strategies are applied to (partially) solve the sudoku.</summary>
        /// <param name="index">The index of the square we want to eliminate the value from.</param>
        /// <param name="value">The value we want to eliminate.</param>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public bool eleminate(int index, int value)
        {
            // If the value is not present in the given square, we can stop here
            if(!sudoku[index].Contains(value))
                return true;

            // Remove the value
            sudoku[index].Remove(value);

            // Check for a contradiction
            if(sudoku[index].Count == 0)
                return false;

            // We'll need a SudokuIndexHelper
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudoku.N);

            // If we have only one value left in our square, we can remove that value from all its peers
            if(sudoku[index].Count == 1)
            {
                int[] peers = sudokuIndexHelper.getPeersFor(index);
                for(int i = 0; i < peers.Length; ++i)
                {
                    if(!eleminate(peers[i], sudoku[index][0]))
                        return false;
                }
            }

            // Check if there is a unit of this square where `value` only occurs once
            // If that is the case, than `value` is the only possible value for that square
            int[,] units = sudokuIndexHelper.getUnitsFor(index);
            for(int i = 0; i < units.GetLength(0); ++i)
            {
                int occurredAtIndex = 0;
                int count = 0;
                for(int j = 0; j < units.GetLength(1); ++j)
                {
                    if(sudoku[units[i, j]].Contains(value))
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
                    if(!assign(occurredAtIndex, value))
                        return false;
                }
            }

            // If we have come here, everything must have gone the right way
            return true;
        }
    }
}
