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
            string sudokuGrid = "4 . . |. . . |8 . 5" +
                                ". 3 . |. . . |. . ." +
                                ". . . |7 . . |. . ." +
                                "------+------+-----" +
                                ". 2 . |. . . |. 6 ." +
                                ". . . |. 8 . |4 . ." +
                                ". . . |. 1 . |. . ." +
                                "------+------+-----" +
                                ". . . |6 . 3 |. 7 ." +
                                "5 . . |2 . . |. . ." +
                                "1 . 4 |. . . |. . .";

            Sudoku sudoku = SudokuReader.readFromString(sudokuGrid, 3);
            printSudoku(sudoku);

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
