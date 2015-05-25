using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Caches the list of neighbors and updates the score delta only of the neighbors that could have changed in the last iteration.</summary>
    class LocalSearcherNeighborList
    {
        /// <summary>The list of neighbors.</summary>
        private List<SwapNeighbor> neighbors;

        /// <summary>Constructor.</summary>
        /// <param name="initialNeighbors">A list containing all possible neighbors for the solution.</param>
        public LocalSearcherNeighborList(List<SwapNeighbor> initialNeighbors)
        { neighbors = initialNeighbors; }

        /// <summary>Property to access the current list of neighbors.</summary>
        public List<SwapNeighbor> Neighbors
        { get { return neighbors; } }

        /// <summary>Update the list of neighbors after the given neighbor is applied.</summary>
        /// <param name="sudoku">The current solution, after applying the given neighbor.</param>
        /// <param name="neighbor">The neighbor that was applied.</param>
        public void update(LocalSudoku sudoku, SwapNeighbor neighbor)
        {
            // We will need the peers of the squares that were swapped
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);

            // Get the peers of the squares that were swapped in the last iteration
            List<int> peers = new List<int>(2 * helper.getPeerCount() + 2);
            if(neighbor != null)
            {
                peers.AddRange(helper.getPeersFor(neighbor.Square1));
                peers.Add(neighbor.Square1);
                peers.AddRange(helper.getPeersFor(neighbor.Square2));
                peers.Add(neighbor.Square2);
            }

            // Loop through all neighbors and update their score delta where necessary
            for(int i = 0; i < neighbors.Count; ++i)
            {
                // Check if this neighbor needs to have its score delta updated
                if(peers.Contains(neighbors[i].Square1) || peers.Contains(neighbors[i].Square2))
                    neighbors[i] = new SwapNeighbor(neighbors[i].Square1, neighbors[i].Square2, sudoku.heuristicDelta(neighbors[i].Square1, neighbors[i].Square2));
            }
        }
    }
}
