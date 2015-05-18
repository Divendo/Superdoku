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
                    if (sudoku[units[SudokuIndexHelper.UNIT_BOX_INDEX, i]].Count == 1)
                        possibleValuesPerBox[box].Remove(sudoku[units[SudokuIndexHelper.UNIT_BOX_INDEX, i]][0]);
                }
            }

            // Now we give each square one of its possibilities as value 
            fixiated = new bool[NN * NN];
            sudokuValues = new int[NN * NN];
            Random random = new Random();

            for(int i = 0; i < NN * NN; ++i)
            {
                if(sudoku[i].Count == 1)
                {
                    sudokuValues[i] = sudoku[i][0];
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
                sudoku[i] = new List<int>(new int[] { sudokuValues[i] });
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

    class LocalSudokuOld
    {
        public int[] values;
        public bool[] isFixed;
        public SudokuIndexHelper helper;
        public List<List<int>> squares;
        public int size;
        public int N;
        public int heuristicValue;
        public Tuple<int, int> changed;

        //the representation used by LocalSearchers
        public LocalSudokuOld(Sudoku sudoku)
        {
            //Set the size
            size = sudoku.NN;
            //set the N
            N = sudoku.N;
            //Create an array of values
            this.values = new int[size * size];
            for (int t = 0; t < values.Length; ++t)
                values[t] = -1;
            //Create an array of bools
            this.isFixed = new bool[size * size];
            //Initialise a helper
            helper = SudokuIndexHelper.get(N);

            //Copy the values and mark them as set
            for(int t = 0; t < values.Length; ++t)
                if (sudoku[t].Count == 1)
                {   values[t] = sudoku[t][0];
                    isFixed[t] = true;}

            //set the squares
            this.setSquares();
            //initialise
            this.initialise();
            //set the heuristic value
            this.setH();
        }

        public LocalSudokuOld(LocalSudokuOld sudoku)
        {
            this.values         = sudoku.values;
            this.helper         = sudoku.helper;
            this.heuristicValue = sudoku.heuristicValue;
            this.isFixed        = sudoku.isFixed;
            this.size           = sudoku.size;
            this.squares        = sudoku.squares;
            this.N              = sudoku.N;
        }

        //initialises the sudoku
        private void initialise()
        {
            //We loop over each of the squares
            foreach(List<int> square in squares)
            {
                int assign = 0;
                Random random = new Random();
                List<int> numbers = new List<int>();
                //We give a value to each of the non-fixed boxes in the square
                for(int t = 0; t < square.Count(); ++t)
                    if(!isFixed[square[t]])
                    {
                        
                        bool valid = false;
                        while (!valid)
                        {   
                            assign = random.Next(0, 9);
                            valid = true;
                            foreach (int index in square)
                                if (values[index] == assign)
                                    valid = false;
                        }
          
                        values[square[t]] = assign;
                    }

            }
        }

        //Retuns a heuristic value based on how many constrains were crossed
        public void setH()
        {
            int result = 0;

            for(int t = 0; t < size * size; ++t)
            {
                result += calculateBrothers(t);
            }
            heuristicValue = result;
        }


        //Returns how many duplicates there are in a box's peers
        private int calculateBrothers(int sample)
        {
            int result = 0;
            int[] peers = helper.getPeersFor(sample);
            foreach (int peer in peers)
                if (peer != sample)
                    if (values[peer] == values[sample])
                        result += 1;
            return result;
        }

        //swaps two values given two indices and updates the heuristic value
        public void swap(int a, int b)
        {
            //Calculate how many constraints these two indices harm
            int firstConstraints = calculateBrothers(a) + calculateBrothers(b);

            //set the tuple
            changed = new Tuple<int, int>(a, b);

            //Swap the values
            int temp = values[a];
            values[a] = values[b];
            values[b] = temp;
            
            //Calculate how many constraints are harmed now
            int secondConstraints = calculateBrothers(a) + calculateBrothers(b);

            //Update the heuristic value
            //why doesn't it woooork
            heuristicValue = heuristicValue + (secondConstraints - firstConstraints);
       
        }

        //returns a string
        public string toString()
        {
            return values.ToString();
        }

        //returns a sudoku
        public Sudoku toSudoku()
        {
            Sudoku result = new Sudoku((int) Math.Sqrt(size));
            for (int t = 0; t < size * size; ++t)
                result.setValue(t, values[t]);
            return result;
        }

        //Create a list of lists containing the indices for the squares
        private void setSquares()
        {
            squares = new List<List<int>>();

            List<int> indices;
            int[,] grid = new int[size, size];
            int n = (int)   Math.Sqrt(size);
            for (int a = 0; a < n; ++a)
                for (int b = 0; b < n; ++b)
                {
                    indices = new List<int>();
                    for (int x = 0; x < n; ++x)
                        for (int y = 0; y < n; ++y)
                            indices.Add((x + a*n) * size + (y+ b*n));
                    squares.Add(indices);
                }
        }
    }
}
