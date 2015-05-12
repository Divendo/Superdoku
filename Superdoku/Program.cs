using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class Program
    {
        static void Main(string[] args)
        {
            /*string sudokuGrid = "4 . . |. . . |8 . 5" +
                                ". 3 . |. . . |. . ." +
                                ". . . |7 . . |. . ." +
                                "------+------+-----" +
                                ". 2 . |. . . |. 6 ." +
                                ". . . |. 8 . |4 . ." +
                                ". . . |. 1 . |. . ." +
                                "------+------+-----" +
                                ". . . |6 . 3 |. 7 ." +
                                "5 . . |2 . . |. . ." +
                                "1 . 4 |. . . |. . .";*/
            string sudokuGrid = "..3.2.6..0..3.5..1..18.64....81.20..7.......8..67.82....26.05..8..2.3..0..5.1.3..";

            Sudoku sudoku = SudokuReader.readFromString(sudokuGrid, 3);
            SudokuHelper sudokuHelper = new SudokuHelper(new Sudoku(3));
            List<int> indices = new List<int>();
            for(int i = 0; i < sudoku.NN * sudoku.NN; ++i)
            {
                if(sudoku[i].Count == 1)
                    indices.Add(i);
            }
            for(int i = 0; i < indices.Count; ++i)
            {
                if(!sudokuHelper.assign(indices[i], sudoku[indices[i]][0]))
                {
                    Console.WriteLine("ERROR!");
                    break;
                }
            }
            printSudoku(sudokuHelper.Sudoku);

            Console.ReadLine();
        }

        static void printSudoku(Sudoku sudoku)
        {
            // Loop through the rows
            for(int y = 0; y < sudoku.NN; ++y)
            {
                // Write a separator when necessary
                if(y != 0 && y % sudoku.N == 0)
                {
                    for(int x = 0; x < sudoku.NN; ++x)
                    {
                        if(x != 0 && x % sudoku.N == 0)
                            Console.Write('+');
                        Console.Write('-');
                    }
                    Console.Write('\n');
                }

                // Loop through the squares in this row
                for(int x = 0; x < sudoku.NN; ++x)
                {
                    // Write a separator when necessary
                    if(x != 0 && x % sudoku.N == 0)
                        Console.Write('|');

                    // Write an x if there are no possibilities for this square
                    // If there is only one possibility, write that possibility
                    // Otherwise we write a '.'
                    List<int> values = sudoku[x, y];
                    if(values.Count == 0)
                        Console.Write('x');
                    else if(values.Count == 1)
                    {
                        if(values[0] <= 9)
                            Console.Write(values[0]);
                        else
                            Console.Write((char) ('A' + (values[0] - 10)));
                    }
                    else
                        Console.Write('.');
                }

                // Next row
                Console.Write('\n');
            }
        }
    }
}
