using PROXX.Domain.Exceptions;

namespace PROXX.Domain
{
    public class Cell
    {
        public const byte maxAdjacentCells = 8;
        public enum CellContent
        {
            BlackHole,
            Empty
        }

        public enum CellVisibility
        {
            Hidden,
            Open,
            Flagged
        }
        public CellContent Content { get; private set; }
        public CellVisibility Visibility { get; set; }

        public Cell(CellContent content, CellVisibility visibility)
        {
            Content = content;
            Visibility = visibility;
        }
        private byte adjacentBlackHoles;

        public byte AdjacentBlackHoles
        {
            get => adjacentBlackHoles;
            set
            {
                if (value > maxAdjacentCells)
                {
                    var errorMessage = string.Format(
                        "Adjacent black holes count cannot be greater than max number of adjacent cells ({0})",
                        maxAdjacentCells);
                    throw new MaxAdjacentCellsExceededException(errorMessage);
                }
                adjacentBlackHoles = value;
            }
        }
    }
}