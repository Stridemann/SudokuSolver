namespace SudokuSolver.Logic
{
    using System.Text;

    public struct CellState
    {
        private const int ALL_POSSIBLE = 0b00000000_00000000_00000011_11111110;

        public CellState(byte finalNumber, int x, int y) : this()
        {
            FinalNumber = finalNumber;
            X = x;
            Y = y;

            if (finalNumber == 0)
            {
                PossibleNumbers = ALL_POSSIBLE;
                PossibleAmount = 9;
            }
            else
            {
                IsDone = true;
            }
        }

        public bool IsDone;
        public byte FinalNumber;
        public int PossibleNumbers = ALL_POSSIBLE;
        public int PossibleAmount;
        public int X;
        public int Y;

        public void UpdateFinalValue()
        {
            for (byte i = 1; i <= 9; i++)
            {
                if ((PossibleNumbers & (1 << i)) != 0)
                {
                    ChoseValue(i);

                    return;
                }
            }
        }

        public void ChoseValue(byte i)
        {
            FinalNumber = i;
            IsDone = true;
        }

        public void WriteState(StringBuilder sb)
        {
            if (IsDone)
                sb.Append(FinalNumber.ToString());
            else
                sb.Append($"[{PossibleNumbers}]");
        }
    }
}
