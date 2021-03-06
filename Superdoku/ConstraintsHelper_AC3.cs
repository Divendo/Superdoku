﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class that helps solving a sudoku using constraints by applying the AC3 algorithm.</summary>
    class ConstraintsHelper_AC3 : ConstraintsHelper
    {
        public ConstraintsHelper_AC3(Sudoku sudoku)
            : base(sudoku) { }

        public override bool assign(int index, ulong value)
        {
            sudoku[index] = value;
            return apply(sudoku, index);
        }

        public override bool clean()
        {
            return apply(sudoku);
        }

        /// <summary>Runs the algorithm on the Sudoku.</summary>
        /// <param name="sudoku">The sudoku to run the algorithm on.</param>
        /// <param name="changedSquare">The square that has been changed, or -1 if all squares should be added to the queue.</param>
        /// <returns>True if successful, false otherwise (e.g. in case a contradiction is reached).</returns>
        public bool apply(Sudoku sudoku, int changedSquare = -1)
        {
            // Nothing to do if changedSquare is set and it has more than 1 possibility left
            if(changedSquare != -1 && sudoku.valueCount(changedSquare) != 1)
                return true;

            // We will need an index helper
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudoku.N);

            // Keep track of the constraints we still need to check
            HashSet<Constraint> toCheck = new HashSet<Constraint>();
            Queue<Constraint> toCheckQueue = new Queue<Constraint>(sudoku.NN * sudoku.NN * (3 * sudoku.NN - 2 * sudoku.N - 1));

            // Add the constraints to the queue
            if(changedSquare != -1)
            {
                int[] peers = sudokuIndexHelper.getPeersFor(changedSquare);
                for(int peer = 0; peer < peers.Length; ++peer)
                {
                    Constraint constraint = new Constraint(peers[peer], changedSquare, sudoku.N);
                    toCheck.Add(constraint);
                    toCheckQueue.Enqueue(constraint);
                }
            }
            else
            {
                for(int square = 0; square < sudoku.NN * sudoku.NN; ++square)
                {
                    int[] peers = sudokuIndexHelper.getPeersFor(square);
                    for(int peer = 0; peer < peers.Length; ++peer)
                    {
                        Constraint constraint = new Constraint(square, peers[peer], sudoku.N);
                        toCheck.Add(constraint);
                        toCheckQueue.Enqueue(constraint);
                    }
                }
            }

            // Keep running while there are still constraints in the queue
            while(toCheckQueue.Count != 0)
            {
                // Increase the iteration count
                ++iterations;

                // Pop the constraint we are going to check
                Constraint constraint = toCheckQueue.Dequeue();
                toCheck.Remove(constraint);

                // If the other square has only one value left, we can eliminate it from the current square
                if(sudoku.valueCount(constraint.square2) == 1 && (sudoku[constraint.square1] & sudoku[constraint.square2]) != 0)
                {
                    // Remove the value from the domain
                    sudoku[constraint.square1] ^= sudoku[constraint.square2];

                    // Check for a contradiction
                    if(sudoku[constraint.square1] == 0)
                        return false;
                    // Only add new constraints to the queue if it is worth checking them
                    else if(sudoku.valueCount(constraint.square1) == 1)
                    {
                        // Add new constraints to the queue
                        int[] peers = sudokuIndexHelper.getPeersFor(constraint.square1);
                        for(int peer = 0; peer < peers.Length; ++peer)
                        {
                            Constraint newConstraint = new Constraint(peers[peer], constraint.square1, sudoku.N);
                            if(!toCheck.Contains(newConstraint))
                            {
                                toCheck.Add(newConstraint);
                                toCheckQueue.Enqueue(newConstraint);
                            }
                        }
                    }
                }
            }

            // If we have come here, everything went successful
            return true;
        }

        /// <summary>A structure representing a constraint.</summary>
        private struct Constraint
        {
            public int square1;
            public int square2;
            private int n;          // Used for creating good hash values

            public Constraint(int square1, int square2, int n)
            {
                this.square1 = square1;
                this.square2 = square2;
                this.n = n;
            }

            public override bool Equals(Object obj)
            {
                if(!(obj is Constraint))
                    return false;

                Constraint other = (Constraint) obj;
                return square1 == other.square1 && square2 == other.square2;
            }

            public bool Equals(Constraint other)
            {
                return square1 == other.square1 && square2 == other.square2;
            }

            public override int GetHashCode()
            {
                if(square1 < square2)
                    return square1 + square2 * n * n * n * n;
                else
                    return -(square1 + square2 * n * n * n * n);
            }
        }
    }

    /// <summary>A factor for the ConstraintsHelper_AC3 class.</summary>
    class ConstraintsHelperFactory_AC3 : ConstraintsHelperFactory
    {
        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelper_AC3(sudoku); }
    }
}
