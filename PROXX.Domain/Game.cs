namespace PROXX.Domain
{
    public class Game
    {
        public enum GameState
        {
            Won,
            Lost,
            InProgress
        }

        public const int maxAllowedLength = 40;
        public GameState State { get; private set; }

        private int blackHolesCount;
        private int cellsToOpen;

        public int FieldLength { get; private set; }

        public Cell[,] Field { get; private set; }

        private List<(int x, int y)> blackHolesCoordinates;

        public Game(int fieldLength, int blackHolesCount)
        {

            if (fieldLength <= 0 || fieldLength > maxAllowedLength)
            {
                var errorMessage = string.Format(
                    "Field length value must be greater than zero and less or equal to {0}", maxAllowedLength);
                throw new ArgumentException(errorMessage);
            }

            this.FieldLength = fieldLength;

            var maxBlackHolesCount = fieldLength * fieldLength - 1;
            if (blackHolesCount <= 0 || blackHolesCount > maxBlackHolesCount)
            {
                var errorMessage = string.Format(
                    "Black holes count must be greater than zero and less or equal to {0}", maxBlackHolesCount);
                throw new ArgumentException(errorMessage);
            }

            this.blackHolesCount = blackHolesCount;

            this.Field = new Cell[fieldLength, fieldLength];
            blackHolesCoordinates = new List<(int x, int y)>();
            this.State = GameState.InProgress;
        }

        public void Initialize()
        {
            var random = new Random();

            for (int i = 0; i < blackHolesCount; i++)
            {
                var x = -1;
                var y = -1;
                while (true)
                {
                    x = random.Next(FieldLength);
                    y = random.Next(FieldLength);

                    if (Field[x, y] != null && Field[x, y].Content == Cell.CellContent.BlackHole)
                        continue;

                    break;
                }

                Field[x, y] = new Cell(Cell.CellContent.BlackHole, Cell.CellVisibility.Hidden);
                blackHolesCoordinates.Add((x, y));
            }

            for (int i = 0; i < FieldLength; i++)
                for (int j = 0; j < FieldLength; j++)
                {
                    if (Field[i, j] == null)
                        Field[i, j] = new Cell(Cell.CellContent.Empty, Cell.CellVisibility.Hidden);
                }


            this.cellsToOpen = FieldLength * FieldLength - blackHolesCount;
        }

        private bool IsBlackHole((int x, int y) coordinates) =>
            Field[coordinates.x, coordinates.y].Content == Cell.CellContent.BlackHole;

        private bool IsInBounds((int x, int y) coordinates)
            => coordinates.x >= 0 && coordinates.x < FieldLength
                && coordinates.y >= 0 && coordinates.y < FieldLength;

        private bool IsHidden((int x, int y) coordinates)
            => Field[coordinates.x, coordinates.y].Visibility == Cell.CellVisibility.Hidden;

        public void Flag((int x, int y) coordinates)
        {
            var currentCell = Field[coordinates.x, coordinates.y];

            if (currentCell.Visibility == Cell.CellVisibility.Hidden)
                currentCell.Visibility = Cell.CellVisibility.Flagged;
        }
        public void UnFlag((int x, int y) coordinates)
        {

            var currentCell = Field[coordinates.x, coordinates.y];

            if (currentCell.Visibility == Cell.CellVisibility.Flagged)
                currentCell.Visibility = Cell.CellVisibility.Hidden;
        }

        public GameState OpenCell((int x, int y) coordinates)
        {
            // if given cell is flagged - cannot open it. Need to unflag it first.
            if (Field[coordinates.x, coordinates.y].Visibility == Cell.CellVisibility.Flagged)
                return State;

            var isBlackHole = IsBlackHole(coordinates);
            if (isBlackHole)
            {
                //change game's state to "lost"
                State = GameState.Lost;

                //show all black holes
                foreach (var c in blackHolesCoordinates)
                    Field[c.x, c.y].Visibility = Cell.CellVisibility.Open;

                return State;
            }

            CheckAdjacentCells(coordinates);

            return State;
        }

        private void CheckAdjacentCells((int x, int y) coordinate)
        {
            //introduce stack to make an iterative algorithm instead of recursive
            var stack = new Stack<(int x, int y)>();
            stack.Push(coordinate);

            while (stack.TryPop(out var c))
            {
                var adjacent = new List<(int x, int y)>();

                var x = c.x;
                var y = c.y;

                var north = (x, y: y + 1);
                if (IsInBounds(north))
                    adjacent.Add(north);

                var south = (x, y: y - 1);
                if (IsInBounds(south))
                    adjacent.Add(south);

                var west = (x: x - 1, y);
                if (IsInBounds(west))
                    adjacent.Add(west);

                var east = (x: x + 1, y);
                if (IsInBounds(east))
                    adjacent.Add(east);

                var northWest = (x: x - 1, y: y + 1);
                if (IsInBounds(northWest))
                    adjacent.Add(northWest);

                var northEast = (x: x + 1, y: y + 1);
                if (IsInBounds(northEast))
                    adjacent.Add(northEast);

                var southWest = (x: x - 1, y: y - 1);
                if (IsInBounds(southWest))
                    adjacent.Add(southWest);

                var southEast = (x: x + 1, y: y - 1);
                if (IsInBounds(southEast))
                    adjacent.Add(southEast);

                var adjacentBlackHoles = default(byte);
                foreach (var ac in adjacent)
                {
                    if (IsBlackHole(ac))
                        adjacentBlackHoles++;
                }

                Field[x, y].AdjacentBlackHoles = adjacentBlackHoles;
                Field[x, y].Visibility = Cell.CellVisibility.Open;
                cellsToOpen--;

                //if there are no more cells to open - the game is won
                if (cellsToOpen == 0)
                {
                    State = GameState.Won;
                    return;
                }

                //if no adjacent black holes - we need to open adjacent cells
                //so we put each adjacent cell on stack it order to check it
                if (adjacentBlackHoles == 0)
                {
                    foreach (var ac in adjacent)
                        if (IsHidden(ac) //must be hidden
                            && !stack.Contains(ac)) //must not be already on stack
                            stack.Push(ac);
                }
            }
        }
    }
}