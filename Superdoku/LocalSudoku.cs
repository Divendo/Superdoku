using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Represents a sudoku that can be used in local search algorithms.</summary>
    class LocalSudoku
    {
        /// <summary>The sudoku that is represented.</summary>
        private int[] sudokuValues;

        /// <summary>A list of booleans telling wheter an index is fixiated.</summary>
        private bool[] fixiated;

        /// <summary>The size of the sudoku (n*n by n*n squares).</summary>
        private int n;

        /// <summary>The heuristic value of this sudoku.</summary>
        private int heuristicValue;

        /// <summary>Construct from a sudoku, copying the squares with one possibility and filling in the rest.</summary>
        /// <param name="sudoku">The sudoku that should be used to initialise.</param>
        public LocalSudoku(Sudoku sudoku)
        {
            // Remember the size
            n = sudoku.N;

            // Create a list of possibly values for each box
            // That is, values that are not fixed yet for some square in that box
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(N);
            List<int>[] possibleValuesPerBox = new List<int>[NN];
            List<int> allValues = new List<int>(NN);
            for(int i = 0; i < NN; ++i)
                allValues.Add(i);
            for(int box = 0; box < NN; ++box)
            {
                possibleValuesPerBox[box] = new List<int>(allValues);

                int[,] units = sudokuIndexHelper.getUnitsFor(N * (box % N), N * (box / N));
                for(int i = 0; i < NN; ++i)
                {
                    int singleValue = sudoku.singleValue(units[SudokuIndexHelper.UNIT_BOX_INDEX, i]);
                    if(singleValue != -1)
                        possibleValuesPerBox[box].Remove(singleValue);
                }
            }

            // Now we give each square one of its possibilities as value 
            fixiated = new bool[NN * NN];
            sudokuValues = new int[NN * NN];
            Random random = new Random();

            for(int i = 0; i < NN * NN; ++i)
            {
                int singleValue = sudoku.singleValue(i);
                if(singleValue != -1)
                {
                    sudokuValues[i] = singleValue;
                    fixiated[i] = true;
                }
                else
                {
                    int box = sudokuIndexHelper.indexToX(i) / N + N * (sudokuIndexHelper.indexToY(i) / N);
                    int index = random.Next(0, possibleValuesPerBox[box].Count());
                    sudokuValues[i] = possibleValuesPerBox[box][index];
                    possibleValuesPerBox[box].RemoveAt(index);
                }
            }

            // Calculate the heuristic value
            calcHeuristicValue();
        }

        /// <summary>Copy constructor, makes a deep copy.</summary>
        public LocalSudoku(LocalSudoku other)
        {
            // Copy all values
            n = other.N;
            sudokuValues = new int[NN * NN];
            fixiated = other.fixiated;

            for(int i = 0; i < NN*NN; ++i)
                sudokuValues[i] = other[i];
            heuristicValue = other.heuristicValue;
        }

        /// <summary>Calculates (or recalculates) the heuristic value of this solution.</summary>
        public void calcHeuristicValue()
        {
            // Reset the heuristic value
            heuristicValue = 0;
            
            // We will use this array to copy all possible values from
            int[] allValues = new int[NN];
            for(int i = 0; i < NN; ++i)
                allValues[i] = i;

            // Loop through all rows and columns and count the amount of missing numbers in each of them
            for(int i = 0; i < NN; ++i)
            {
                HashSet<int> row = new HashSet<int>(allValues);
                HashSet<int> col = new HashSet<int>(allValues);

                for(int j = 0; j < NN; ++j)
                {
                    row.Remove(sudokuValues[j + i * NN]);
                    col.Remove(sudokuValues[i + j * NN]);
                }
      
                heuristicValue += row.Count;
                heuristicValue += col.Count;
            }
        }

        /// <summary>Determines how much the heuristic value will change if the squares with the given indices are swapped.</summary>
        /// <param name="index1">The index of the first square.</param>
        /// <param name="index2">The index of the second square.</param>
        /// <returns>How much the heuristic value will change.</returns>
        public int heuristicDelta(int index1, int index2)
        {
            // If we ar not swapping anything, nothing will happen
            if(index1 == index2)
                return 0;

            // Get the coordinates of the squares
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(N);
            int x1 = sudokuIndexHelper.indexToX(index1);
            int y1 = sudokuIndexHelper.indexToY(index1);
            int x2 = sudokuIndexHelper.indexToX(index2);
            int y2 = sudokuIndexHelper.indexToY(index2);

            // We are going to keep track of the change in value in this variable
            int delta = 0;

            // We will use this array to copy all possible values from
            int[] allValues = new int[NN];
            for(int i = 0; i < NN; ++i)
                allValues[i] = i;

            // If the squares are not in the same row, we calculate how much the values of their rows would change
            if(y1 != y2)
            {
                // Calculate the score of the rows before swapping
                HashSet<int> row1 = new HashSet<int>(allValues);
                HashSet<int> row2 = new HashSet<int>(allValues);
                for(int i = 0; i < NN; ++i)
                {
                    row1.Remove(sudokuValues[i + y1 * NN]);
                    row2.Remove(sudokuValues[i + y2 * NN]);
                }
                int before = row1.Count + row2.Count;

                // Calculate the score of the rows after swapping
                row1 = new HashSet<int>(allValues);
                row2 = new HashSet<int>(allValues);
                for(int i = 0; i < NN; ++i)
                {
                    if(i + y1 * NN == index1)
                        row1.Remove(sudokuValues[index2]);
                    else
                        row1.Remove(sudokuValues[i + y1 * NN]);

                    if(i + y2 * NN == index2)
                        row2.Remove(sudokuValues[index1]);
                    else
                        row2.Remove(sudokuValues[i + y2 * NN]);
                }

                // Calculate the change in score
                delta += row1.Count + row2.Count - before;
            }

            // Do the same thing if the squares are not in the same column
            if(x1 != x2)
            {
                // Calculate the score of the rows before swapping
                HashSet<int> col1 = new HashSet<int>(allValues);
                HashSet<int> col2 = new HashSet<int>(allValues);
                for(int i = 0; i < NN; ++i)
                {
                    col1.Remove(sudokuValues[x1 + i * NN]);
                    col2.Remove(sudokuValues[x2 + i * NN]);
                }
                int before = col1.Count + col2.Count;

                // Calculate the score of the rows after swapping
                col1 = new HashSet<int>(allValues);
                col2 = new HashSet<int>(allValues);
                for(int i = 0; i < NN; ++i)
                {
                    if(x1 + i * NN == index1)
                        col1.Remove(sudokuValues[index2]);
                    else
                        col1.Remove(sudokuValues[x1 + i * NN]);

                    if(x2 + i * NN == index2)
                        col2.Remove(sudokuValues[index1]);
                    else
                        col2.Remove(sudokuValues[x2 + i * NN]);
                }

                // Calculate the change in score
                delta += col1.Count + col2.Count - before;
            }

            // Return the result
            return delta;
        }

        /// <summary>Swaps the two given squares.</summary>
        /// <param name="index1">The first square.</param>
        /// <param name="index2">The second square.</param>
        public void swap(int index1, int index2)
        {
            // Update the heuristic value
            heuristicValue += heuristicDelta(index1, index2);

            // Actually swap the squares
            int tmp = sudokuValues[index1];
            sudokuValues[index1] = sudokuValues[index2];
            sudokuValues[index2] = tmp;
        }

        /// <summary>Returns the sudoku that is represented by this instance.</summary>
        /// <returns>The sudoku that is represented by this instance.</returns>
        public Sudoku toSudoku()
        {
            Sudoku sudoku = new Sudoku(N);
            for(int i = 0; i < NN * NN; ++i)
                sudoku[i] = 1ul << sudokuValues[i];
            return sudoku;
        }

        /// <summary>Returns whether or not a certain square is fixed.</summary>
        /// <param name="index">The square that is to be checked.</param>
        /// <returns>True if the square is fixed, false otherwise</returns>
        public bool isFixed(int index)
        { return fixiated[index]; }

        /// <summary>The index operator to access the values in the squares.</summary>
        /// <param name="index">The index of the square whose value you want to retrieve.</param>
        /// <returns>The value of the given square.</returns>
        public int this[int index]
        {
            get { return sudokuValues[index]; }
            set { sudokuValues[index] = value; }
        }

        /// <summary>The index operator to access the values in the squares.</summary>
        /// <param name="x">The x-coordinate of the square whose value you want to retrieve.</param>
        /// <param name="y">The y-coordinate of the square whose value you want to retrieve.</param>
        /// <returns>The value of the given square.</returns>
        public int this[int x, int y]
        {
            get { return sudokuValues[x + y * NN]; }
            set { sudokuValues[x + y * NN] = value; }
        }
        
        /// <summary>Returns wheter an index is fixed or not</summary>
        public bool[] Fixed
        { get { return fixiated; } }

        /// <summary>The size of the sudoku (n*n by n*n squares).</summary>
        public int N
        { get { return n; } }

        /// <summary>Convenenience property, equals N*N.</summary>
        public int NN
        { get { return n * n; } }

        /// <summary>The heuristic value of this solution.</summary>
        public int HeuristicValue
        { get { return heuristicValue; } }
    }
}
