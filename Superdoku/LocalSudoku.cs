using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class LocalSudoku
    {
        public int[] values;
        public bool[] isFixed;
        public SudokuIndexHelper helper;
        public List<List<int>> squares;
        public int size;
        public int N;
        public int heuristicValue;

        //the representation used by LocalSearchers
        public LocalSudoku(Sudoku sudoku)
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

        public LocalSudoku(LocalSudoku sudoku)
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

                //We give a value to each of the non-fixed boxes in the square
                for(int t = 0; t < square.Count(); ++t)
                    if(!isFixed[square[t]])
                    {
                        bool valid = false;
                        while (!valid)
                        {
                            valid = true;
                            foreach (int index in square)
                                if (values[index] == assign)
                                {
                                    valid = false;
                                    assign++;
                                }
                        }
          
                        values[square[t]] = assign;
                    }

            }
        }

        //Retuns a heuristic value based on how many constrains were crossed
        private void setH()
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

            //Swap the values
            int temp = values[a];
            values[a] = values[b];
            values[b] = temp;
            
            //Calculate how many constraints are harmed now
            int secondConstraints = calculateBrothers(a) + calculateBrothers(b);

            //Update the heuristic value
            //heuristicValue = heuristicValue + (secondConstraints - firstConstraints);
            this.setH();
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
