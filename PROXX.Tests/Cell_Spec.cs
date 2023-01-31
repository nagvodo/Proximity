using PROXX.Domain;
using PROXX.Domain.Exceptions;
using Xunit;

namespace PROXX.Tests
{
    //unit tests for Cell
    public class Cell_Spec
    {
        [Fact]
        public void Adjacent_black_holes_count_cannot_be_more_than_max_adjacent_cells()
        {
            //Arrange
            var content = Cell.CellContent.Empty;
            var visibility = Cell.CellVisibility.Open;
            var cell = new Cell(content, visibility)
            {
                AdjacentBlackHoles = Cell.maxAdjacentCells
            };

            //Assert
            Assert.Throws<MaxAdjacentCellsExceededException>(() => cell.AdjacentBlackHoles++);
        }
    }
}
