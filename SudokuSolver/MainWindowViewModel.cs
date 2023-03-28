namespace SudokuSolver
{
    using System.Collections.ObjectModel;
    using Logic;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            var sudoku = SudokuPuzzleProvider.Simple;

            for (var y = 0; y < 9; y++)
            {
                for (var x = 0; x < 9; x++)
                {
                    Cells.Add(new Cell(sudoku[y * 9 + x]));
                }
            }

            var solutions = Sudoku.Solve(sudoku);

            if (solutions.Count > 0)
            {
                Update(solutions[2].Cells);
            }
        }

        private void Update(CellState[,] cells)
        {
            for (var y = 0; y < 9; y++)
            {
                for (var x = 0; x < 9; x++)
                {
                    var cellVal = cells[x, y];

                    if (cellVal.IsDone)
                    {
                        Cells[y * 9 + x].DisplayValue = cellVal.FinalNumber.ToString();
                    }
                    else
                    {
                        var result = "";

                        for (int i = 1; i <= 9; i++)
                        {
                            if ((cellVal.PossibleNumbers & (1 << i)) != 0)
                            {
                                if (!string.IsNullOrEmpty(result))
                                    result += "|";

                                result += i.ToString();
                            }
                        }

                        Cells[y * 9 + x].DisplayValue = $"[{result}]";
                    }
                }
            }
        }

        public ObservableCollection<Cell> Cells { get; } = new ObservableCollection<Cell>();
    }

    public class Cell : BindableBase
    {
        private int _cellValue;
        private string _displayValue;

        public Cell(int cellValue)
        {
            CellValue = cellValue;
            DisplayValue = cellValue == 0 ? "" : cellValue.ToString();
        }

        public int CellValue
        {
            get => _cellValue;
            set => SetProperty(ref _cellValue, value);
        }

        public string DisplayValue
        {
            get => _displayValue;
            set => SetProperty(ref _displayValue, value);
        }
    }
}
