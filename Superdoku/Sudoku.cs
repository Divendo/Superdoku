﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class represents a (partially) filled in sudoku.</summary>
    class Sudoku
    {
        /// <summary>The size of the sudoku (n*n by n*n squares).</summary>
        private int n;

        /// <summary>
        /// Lists the possible values for each square. Each square is indicated by an index ranging from 0 to n^4 - 1 (inclusive).
        /// The index of a square can be expressed in its zero-based (x,y) coordinates: index = n*n * y + x.
        /// Each value ranges from 0 to n*n - 1 (inclusive), note that this means that for a standard sudoku we work with the values 0 to 8 here.
        /// </summary>
        private List<int>[] values;

        /// <summary>Constructor, constructs a sudoku where everything is still possible.</summary>
        /// <param name="n">The size of the sudoku (n*n by n*n squares).</param>
        public Sudoku(int n)
        {
            // Remember the size
            this.n = n;

            // Initialise the values
            values = new List<int>[NN * NN];
            List<int> all = new List<int>(NN);
            for(int i = 0; i < NN; ++i)
                all.Add(i);
            for(int i = 0; i < NN * NN; ++i)
                values[i] = new List<int>(all);
        }


        /// <summary>Copy constructor, makes a deep copy of the given sudoku.</summary>
        /// <param name="sudoku">A deep copy is made of this sudoku.</param>
        public Sudoku(Sudoku sudoku)
        {
            // Remember the size
            this.n = sudoku.n;

            // Copy the values
            values = new List<int>[NN * NN];
            for(int i = 0; i < NN * NN; ++i)
                values[i] = new List<int>(sudoku[i]);
        }

        /// <summary>Sets the given square to a single possibility.</summary>
        /// <param name="index">The index of the square to set.</param>
        /// <param name="value">The value that should become the only possibility for the square.</param>
        public void setValue(int index, int value)
        { values[index] = new List<int>(new int[] {value}); }

        /// <summary>Sets the given square to a single possibility.</summary>
        /// <param name="x">The x-coordinate of the square to set.</param>
        /// <param name="y">The y-coordinate of the square to set.</param>
        /// <param name="value">The value that should become the only possibility for the square.</param>
        public void setValue(int x, int y, int value)
        { values[y * NN + x] = new List<int>(new int[] { value }); }

        /// <summary>Perform a simple check whether or not the sudoku is solved. This check involves only checking if all squares have exactly one possible value.</summary>
        /// <returns>Whether or not the sudoku is solved.</returns>
        public bool isSolvedSimple()
        {
            for(int i = 0; i < values.Length; ++i)
            {
                if(values[i].Count != 1)
                    return false;
            }

            return true;
        }

        /// <summary>Performs a thorough check whether or not the sudoku has been solved. All squares are checked to have exactly one value, and the values of the sqaures are checked to be correct.</summary>
        /// <returns>Whether or not the sudoku is solved.</returns>
        public bool isSolved()
        {
            // Check if every square contains exactly one possible value
            if(!isSolvedSimple())
                return false;

            // Check for every square if its value does not occur in its peers
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(n);
            for(int i = 0; i < values.Length; ++i)
            {
                int[] peers = sudokuIndexHelper.getPeersFor(i);
                for(int j = 0; j < peers.Length; ++j)
                {
                    if(values[peers[j]][0] == values[i][0])
                        return false;
                }
            }

            // If we have come here, we have passed all checks
            return true;
        }

        /// <summary>Checks if the sudoku is still consistent. That is: no squares without any possibilities and no two squares with the same single possibility in one unit.</summary>
        /// <returns>True if the sudoku is consistent, false otherwise.</returns>
        public bool isConsistent()
        {
            // We need a sudoku index helper
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(n);

            // Loop through all squares
            for(int square = 0; square < NN * NN; ++square)
            {
                // Check for empty squares
                if(values[square].Count == 0)
                    return false;

                // If this square has a single possibility, check its peers for another square with that single possibility
                if(values[square].Count == 1)
                {
                    int[] peers = sudokuIndexHelper.getPeersFor(square);
                    for(int peer = 0; peer < peers.Length; ++peer)
                    {
                        if(values[peers[peer]].Count == 1 && values[peers[peer]] == values[square])
                            return false;
                    }
                }
            }

            // If we have come here all squares have been checked and no inconsistencies have been found
            return true;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Sudoku))
                return false;

            return Equals((Sudoku)obj);
        }
        
        public bool Equals(Sudoku other)
        {
            // Are we the same size?
            if(n != other.n)
                return false;

            // First check if every square contains the same amount of values
            for(int square = 0; square < NN * NN; ++square)
            {
                if(values[square].Count != other.values[square].Count)
                    return false;
            }

            // Now check if each square matches
            for(int square = 0; square < NN * NN; ++square)
            {
                for(int value = 0; value < values[square].Count; ++value)
                {
                    if(!other.values[square].Contains(values[square][value]))
                        return false;
                }
            }

            // If we have come here, all checks have passed and we are the same sudoku
            return true;
        }

        private int getSquareHash(int index)
        {
            List<int> squareValues = values[index];
            int result = 0;
            for(int i = 0; i < squareValues.Count; ++i)
                result ^= (1 << squareValues[i]);
            return result;
        }

        public override int GetHashCode()
        {
            int result = 0;
            for(int i = 0; i < NN; ++i)
                result ^= (getSquareHash(N * (i % N) + NN * N * (i / N)) << i);
            return result;
        }

        /// <summary>The index operator to access the values in the squares.</summary>
        /// <param name="index">The index of the square whose values you want to retrieve.</param>
        /// <returns>A list of possible values for the given square.</returns>
        public List<int> this[int index]
        {
            get { return values[index]; }
            set { values[index] = value; }
        }

        /// <summary>The index operator to access the values in the squares.</summary>
        /// <param name="x">The x-coordinate of the square whose values you want to retrieve.</param>
        /// <param name="y">The y-coordinate of the square whose values you want to retrieve.</param>
        /// <returns>A list of possible values for the given square.</returns>
        public List<int> this[int x, int y]
        {
            get { return values[x + y * NN]; }
            set { values[x + y * NN] = value; }
        }

        /// <summary>The size of the sudoku (n*n by n*n squares).</summary>
        public int N
        { get { return n; } }

        /// <summary>Convenenience property, equals N*N.</summary>
        public int NN
        { get { return n * n; } }
    }

    /// <summary>This class contains helper functions for the indices of the squares.</summary>
    class SudokuIndexHelper
    {
        /// <summary>The size of the sudoku this helper represents (n*n by n*n squares).</summary>
        private int n;
        /// <summary>An array containing the indices of the peers for each square.</summary>
        private int[][] peers;
        /// <summary>An array containing the units for each square.</summary>
        private int[][,] units;

        /// <summary>The index of the column unit in the array of units for a square.</summary>
        public const int UNIT_COLUMN_INDEX = 0;
        /// <summary>The index of the row unit in the array of units for a square.</summary>
        public const int UNIT_ROW_INDEX = 1;
        /// <summary>The index of the box unit in the array of units for a square.</summary>
        public const int UNIT_BOX_INDEX = 2;

        /// <summary>The existing sudoku index helpers.</summary>
        private static Dictionary<int, SudokuIndexHelper> sudokuIndexHelpers = new Dictionary<int, SudokuIndexHelper>();

        /// <summary>Returns a SudokuIndexHelper for the given size.</summary>
        /// <param name="n">The size of the sudoku this helper represents (n*n by n*n squares).</param>
        /// <returns>The requested SudokuIndexHelper.</returns>
        public static SudokuIndexHelper get(int n)
        {
            if(!sudokuIndexHelpers.ContainsKey(n))
                sudokuIndexHelpers[n] = new SudokuIndexHelper(n);
            return sudokuIndexHelpers[n];
        }

        /// <summary>Needs to be called once before using this class.</summary>
        private SudokuIndexHelper(int n)
        {
            // Remember the size
            this.n = n;

            // Initialise the units and peers
            peers = new int[NN * NN][];
            units = new int[NN * NN][,];
            for(int x = 0; x < NN; ++x)
            {
                for(int y = 0; y < NN; ++y)
                {
                    units[x + y * NN] = new int[3, NN];
                    HashSet<int> peerSet = new HashSet<int>();

                    // Needed for the box unit
                    int leftX = (x / n) * n;
                    int topY = (y / n) * n;

                    for(int i = 0; i < NN; ++i)
                    {
                        // The column unit
                        units[x + y * NN][UNIT_COLUMN_INDEX, i] = i * NN + x;
                        // The row unit
                        units[x + y * NN][UNIT_ROW_INDEX, i] = y * NN + i;
                        // The box unit
                        units[x + y * NN][UNIT_BOX_INDEX, i] = (topY + i / n) * NN + (leftX + i % n);

                        // Add all these squares to the peers
                        peerSet.Add(i * NN + x);
                        peerSet.Add(y * NN + i);
                        peerSet.Add((topY + i / n) * NN + (leftX + i % n));
                    }

                    // Remove the square itself from its peers and copy the result
                    peerSet.Remove(x + y * NN);
                    peers[x + y * NN] = new int[n * n - 1 + 2 * (n - 1) * n];
                    peerSet.CopyTo(peers[x + y * NN]);
                }
            }
        }

        /// <summary>Returns the index of the square with the given coordinates.</summary>
        /// <param name="x">The x coordinate of the square.</param>
        /// <param name="y">The y coordinate of the square.</param>
        /// <returns>The index of the square.</returns>
        public int toIndex(int x, int y)
        { return x + y * NN; }

        /// <summary>Returns the x coordinate of the square with the given index.</summary>
        /// <param name="index">The index to convert to an x coordinate.</param>
        /// <returns>The x coordinate of the square.</returns>
        public int indexToX(int index)
        { return index % NN; }

        /// <summary>Returns the y coordinate of the square with the given index.</summary>
        /// <param name="index">The index to convert to an y coordinate.</param>
        /// <returns>The y coordinate of the square.</returns>
        public int indexToY(int index)
        { return index / NN; }

        /// <summary>Returns the peers for the given square.</summary>
        /// <param name="index">The index of the square whose peers should be returned.</param>
        /// <returns>An array containing the indices of the peers of the square.</returns>
        public int[] getPeersFor(int index)
        { return peers[index]; }

        /// <summary>Returns the peers for the given square.</summary>
        /// <param name="x">The x-coordinate of the square.</param>
        /// <param name="y">The y-coordinate of the square.</param>
        /// <returns>An array containing the indices of the peers of the square.</returns>
        public int[] getPeersFor(int x, int y)
        { return getPeersFor(x + y * NN); }

        /// <summary>Returns the units for the given square.</summary>
        /// <param name="index">The index of the square whose units should be returned.</param>
        /// <returns>The units of the square.</returns>
        public int[,] getUnitsFor(int index)
        { return units[index]; }

        /// <summary>Returns the units for the given square.</summary>
        /// <param name="x">The x-coordinate of the square.</param>
        /// <param name="y">The y-coordinate of the square.</param>
        /// <returns>The units of the square.</returns>
        public int[,] getUnitsFor(int x, int y)
        { return getUnitsFor(x + y * NN); }

        /// <summary>The size of the sudoku this helper represents (n*n by n*n squares).</summary>
        public int N
        { get { return n; } }

        /// <summary>Convenenience property, equals N*N.</summary>
        public int NN
        { get { return n * n; } }
    }
}
