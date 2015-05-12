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
            SudokuIndexHelper helper = SudokuIndexHelper.get(3);
            Sudoku sudoku = new Sudoku(3);

            int x = 4;
            int y = 4;

            sudoku[x, y].Clear();
            sudoku[x, y].Add(0);
            int[,] units = helper.getUnitsFor(x, y);
            for(int i = 0; i < 3; ++i)
            {
                Sudoku su = new Sudoku(sudoku);
                for(int j = 0; j < su.NN; ++j)
                {
                    if(units[i, j] != y * su.NN + x)
                        su[units[i, j]].Clear();
                }
                printSudoku(su);
                Console.WriteLine();
            }

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
                        Console.Write(values[0]);
                    else
                        Console.Write('.');
                }

                // Next row
                Console.Write('\n');
            }
        }
    }
}
