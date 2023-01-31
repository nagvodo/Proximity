using PROXX.Domain;
using Xunit;
using static PROXX.Domain.Game;

namespace PROXX.Tests
{
    public class Game_Spec
    {
        [Fact]
        public void Cannot_create_a_game_with_board_length_more_than_max()
        {
            //Arrange
            var badLength = Game.maxAllowedLength + 1;
            var blackHolesCount = 10;

            //Assert
            Assert.Throws<ArgumentException>(() => new Game(badLength, blackHolesCount));
        }

        [Fact]
        public void Cannot_create_a_game_with_board_length_less_than_1()
        {
            //Arrange
            var badLength = 0;
            var blackHolesCount = 10;

            //Assert
            Assert.Throws<ArgumentException>(() => new Game(badLength, blackHolesCount));
        }

        [Fact]
        public void Cannot_create_a_game_with_number_of_black_holes_more_than_board_length_squared_minus_1()
        {
            //Arrange
            var length = 20;
            var badBlackHolesCount = length * length;

            //Assert
            Assert.Throws<ArgumentException>(() => new Game(length, badBlackHolesCount));
        }

        [Fact]
        public void Cannot_create_a_game_with_number_of_black_holes_less_than_1()
        {
            //Arrange
            var length = 20;
            var badBlackHolesCount = 0;

            //Assert
            Assert.Throws<ArgumentException>(() => new Game(length, badBlackHolesCount));
        }

        [Fact]
        public void The_game_is_lost_when_black_hole_is_opened()
        {
            //Arrange
            var game = new Game(10, 10);
            game.Initialize();
            var state = game.State;
            var blackHoleOpened = false;

            //Act
            for (int i = 0; i < game.FieldLength; i++)
            {
                for (int j = 0; j < game.FieldLength; j++)
                    if (game.Field[i, j].Content == Cell.CellContent.BlackHole)
                    {
                        blackHoleOpened = true;
                        state = game.OpenCell((x: i, y: j));
                        break;
                    }

                if (blackHoleOpened)
                    break;
            }

            //Assert
            Assert.True(state == GameState.Lost);
        }

        [Fact]
        public void The_game_is_in_progress_if_empty_cell_is_opened()
        {
            //Arrange
            var game = new Game(10, 10);
            game.Initialize();
            var state = game.State;
            var emptyCellOpened = false;

            //Act
            for (int i = 0; i < game.FieldLength; i++)
            {
                for (int j = 0; j < game.FieldLength; j++)
                    if (game.Field[i, j].Content == Cell.CellContent.Empty)
                    {
                        state = game.OpenCell((x: i, y: j));
                        emptyCellOpened = true;
                        break;
                    }

                if (emptyCellOpened == true)
                    break;
            }

            //Assert
            Assert.True(state == GameState.InProgress);

        }

        [Fact]
        public void The_game_is_won_when_all_empty_cells_are_opened()
        {
            //Arrange
            var length = 10;
            var blackHolesCount = 10;
            var game = new Game(length, blackHolesCount);
            game.Initialize();

            //Act
            for (int i = 0; i < game.FieldLength; i++)
                for (int j = 0; j < game.FieldLength; j++)
                    if (game.Field[i, j].Content == Cell.CellContent.Empty)
                        game.OpenCell((x: i, y: j));

            //Assert
            Assert.True(game.State == GameState.Won);
        }

        [Fact]
        public void Flagged_cell_cannot_be_opened_and_will_remain_flagged()
        {
            //Arrange
            var game = new Game(10, 10);
            game.Initialize();
            var state = game.State;
            var coordinates = (x: 4, y: 4);

            //Act
            game.Flag(coordinates);
            game.OpenCell(coordinates);

            //Assert
            Assert.True(game.Field[coordinates.x, coordinates.y].Visibility == Cell.CellVisibility.Flagged);
        }

        [Fact]
        public void Flagged_cell_will_not_open_automatically_when_empty_cell_is_being_opened()
        {
            //Arrange
            var game = new Game(10, 10);
            game.Initialize();
            var state = game.State;
            var coordinates = (x: -1, y: -1);
            var random = new Random();
            while (true) //get random empty cell
            {
                var randX = random.Next(0, 9);
                var randY = random.Next(0, 9);

                if (game.Field[randX, randY].Content == Cell.CellContent.Empty)
                {
                    coordinates = (x: randX, y: randY);
                    break;
                }

            }

            //Act
            game.OpenCell(coordinates); //open this cell
            for (int i = 0; i < game.FieldLength; i++) //flag all other cells
            {
                for (int j = 0; j < game.FieldLength; j++)
                {
                    var c = (x: i, y: j);
                    if (c != coordinates)
                        game.Flag(c);
                }
            }


            //Assert
            for (int i = 0; i < game.FieldLength; i++)
            {
                for (int j = 0; j < game.FieldLength; j++)
                {
                    var c = (x: i, y: j);
                    if (c != coordinates)
                        Assert.True(game.Field[c.x, c.y].Visibility == Cell.CellVisibility.Flagged);
                }
            }
        }
    }
}