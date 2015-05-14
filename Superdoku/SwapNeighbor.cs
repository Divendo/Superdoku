using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class SwapNeighbor
    {
        private int delta;
        private int first;
        private int second;
        private int firstValue;
        private int secondValue;

        public SwapNeighbor(LocalSudoku sudoku, int index_a, int index_b)
        {
            first = index_a;
            second = index_b;
            delta = sudoku.heuristicDelta(index_a, index_b);
            firstValue = sudoku[first];
            secondValue = sudoku[second];
        }

        public int Delta
        { get { return delta; } }

        public int First
        { get { return first; } }

        public int Second
        { get { return second; } }

        public static bool equal(SwapNeighbor a, SwapNeighbor b)
        {
            if (a == null)
                return false;

            if (b == null)
                return false;

            return (a.first == b.first && a.second == b.second) && a.firstValue == b.firstValue && a.secondValue == b.secondValue ||
                    (a.first == b.Second && a.second == b.first && a.firstValue == b.secondValue && a.secondValue == b.firstValue);
        }
    }
}
