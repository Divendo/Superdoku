using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Represents a possible swap that can be performed to get a neighbor.</summary>
    class SwapNeighbor
    {
        /// <summary>The first square that should be swapped.</summary>
        private int square1;
        /// <summary>The second square that should be swapped.</summary>
        private int square2;
        /// <summary>The change in score after swapping the squares.</summary>
        private int scoreDelta;

        public SwapNeighbor(int square1, int square2, int scoreDelta)
        {
            this.square1 = square1;
            this.square2 = square2;
            this.scoreDelta = scoreDelta;
        }

        public int ScoreDelta
        { get { return scoreDelta; } }

        public int Square1
        { get { return square1; } }

        public int Square2
        { get { return square2; } }

        public override bool Equals(object obj)
        {
            if(!(obj is SwapNeighbor))
                return false;

            return Equals((SwapNeighbor) obj);
        }
        
        public bool Equals(SwapNeighbor other)
        {
            return square1 == other.square1 && square2 == other.square2 && scoreDelta == other.scoreDelta;
        }

        public override int GetHashCode()
        {
            return square1 * 1000000 + scoreDelta * 10000 + square2;
        }

        public static bool equal(SwapNeighbor a, SwapNeighbor b)
        { return a.Equals(b); }
    }
}
