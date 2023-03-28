namespace SudokuSolver.Logic
{
    using System.Collections.Generic;

    public static class Sudoku
    {
        public static List<SudokuState> Solve(byte[] data)
        {
            var result = new List<SudokuState>();
            var passedStates = new HashSet<string>();
            var initialState = new SudokuState(data);
            passedStates.Add(initialState.GetHash());

            if (initialState.Solve())
            {
                result.Add(initialState);
            }

            var queue = new Queue<SudokuState>();
            queue.Enqueue(initialState);
            var calcs = 0;

            while (queue.TryDequeue(out var state))
            {
                foreach (var mutatedState in state.GetPossibleStateMutations())
                {
                    var hash = mutatedState.GetHash();

                    if (passedStates.Contains(hash))
                        continue;
                    passedStates.Add(hash);
                    calcs++;

                    if (mutatedState.Solve())
                    {
                        result.Add(mutatedState);
                    }
                }
            }

            return result;
        }
    }
}
