namespace SudokuSolver.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using System.Text;

    public class SudokuState
    {
        public SudokuState(byte[] data)
        {
            Cells = new CellState[9, 9];

            for (var y = 0; y < 9; y++)
            {
                for (var x = 0; x < 9; x++)
                {
                    Cells[x, y] = new CellState(data[y * 9 + x], x, y);
                }
            }
        }

        public SudokuState(CellState[,] cells)
        {
            Cells = cells;
        }

        public CellState[,] Cells { get; }

        public bool Solve()
        {
            bool solved;
            bool changed;

            do
            {
                changed = false;
                solved = true;
                
                for (var y = 0; y < 9; y++)
                {
                    for (var x = 0; x < 9; x++)
                    {
                        var cellData = Cells[x, y];

                        if (cellData.IsDone)
                        {
                            changed = RemovePossibleVertically(x, cellData.FinalNumber) || changed;
                            changed = RemovePossibleHorisontally(y, cellData.FinalNumber) || changed;
                            changed = RemovePossibleInSector(x, y, cellData.FinalNumber) || changed;
                        }

                        if (!Cells[x, y].IsDone)
                        {
                            solved = false;
                        }
                    }
                }
            } while (changed);

            return solved;
        }

        public IEnumerable<SudokuState> GetPossibleStateMutations()
        {
            foreach (var cellState in Cells)
            {
                if (cellState.IsDone)
                {
                    continue;
                }

                for (byte i = 1; i <= 9; i++)
                {
                    if ((cellState.PossibleNumbers & (1 << i)) != 0)
                    {
                        yield return Mutate(cellState.X, cellState.Y, i);
                    }
                }
            }
        }

        public SudokuState Mutate(int cellX, int cellY, byte cellChosedValue)
        {
            var newCells = (CellState[,])Cells.Clone();
            newCells[cellX, cellY].ChoseValue(cellChosedValue);
            var newState = new SudokuState(newCells);

            return newState;
        }

        [Pure]
        public string GetHash()
        {
            var sb = new StringBuilder();

            foreach (var state in Cells)
            {
                state.WriteState(sb);
            }

            return sb.ToString();
        }

        private bool RemovePossibleVertically(int x, byte value)
        {
            var shortInvVal = ~(1 << value);
            var changed = false;

            for (var y = 0; y < 9; y++)
            {
                changed = RemovePossibleInCell(x, y, shortInvVal) || changed;
                ;
            }

            return changed;
        }

        private bool RemovePossibleHorisontally(int y, byte value)
        {
            var shortInvVal = ~(1 << value);
            var changed = false;

            for (var x = 0; x < 9; x++)
            {
                changed = RemovePossibleInCell(x, y, shortInvVal) || changed;
            }

            return changed;
        }

        private bool RemovePossibleInSector(int x, int y, byte value)
        {
            var sectorStartX = x / 3 * 3;
            var sectorStartY = y / 3 * 3;
            var shortInvVal = ~(1 << value);
            var changed = false;

            for (var startX = sectorStartX; startX < sectorStartX + 3; startX++)
            {
                for (var startY = sectorStartY; startY < sectorStartY + 3; startY++)
                {
                    changed = RemovePossibleInCell(startX, startY, shortInvVal) || changed;
                }
            }

            return changed;
        }

        private bool RemovePossibleInCell(int x, int y, int shortInvVal)
        {
            var cellData = Cells[x, y];

            if (cellData.IsDone)
            {
                return false;
            }

            var oldPossible = cellData.PossibleNumbers;
            cellData.PossibleNumbers &= shortInvVal;

            if (cellData.PossibleNumbers != oldPossible)
            {
                cellData.PossibleAmount--;

                if (cellData.PossibleAmount == 1)
                {
                    cellData.UpdateFinalValue();
                }

                Cells[x, y] = cellData;

                return true;
            }

            return false;
        }
    }
}
