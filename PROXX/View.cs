using System;
using System.Linq;
using PROXX.Domain;
using static System.Console;

namespace PROXX
{
    public class View
    {
        private const char space = ' ';
        private const string newLine = "\r\n";
        private const char blackHole = 'b';
        private const char hidden = '*';
        private const char flagged = 'f';
        private const char verticalBorder = '|';
        private const string horisontalBorder = "––";
        public Game Game { get; private set; }

        public View(int fieldLength, int blackHolesCount)
        {
            Game = new Game(fieldLength, blackHolesCount);
            Game.Initialize();
        }

        public void DrawPlayingField()
        {
            for (int y = Game.FieldLength - 1; y >= -2; y--)
            {
                for (int x = -2; x < Game.FieldLength; x++)
                {
                    if (y == -2)
                    {
                        if (x >= 0)
                            Write(x % 10);

                        Write(space);
                        continue;
                    }
                    else if (x == -2)
                    {
                        if (y >= 0)
                            Write(y % 10);
                        else
                            Write(space);

                        continue;
                    }
                    else if (y == -1)
                    {
                        Write(horisontalBorder);
                        continue;
                    }
                    else if (x == -1)
                    {
                        Write(verticalBorder);
                        continue;
                    }

                    var cell = Game.Field[x, y];
                    var currentColor = ForegroundColor;
                    switch (cell.Visibility)
                    {
                        case Cell.CellVisibility.Open:
                            if (cell.Content == Cell.CellContent.Empty)
                            {
                                if (cell.AdjacentBlackHoles > 0)
                                    ForegroundColor = ConsoleColor.DarkBlue;

                                Write(cell.AdjacentBlackHoles);
                            }
                            else if (cell.Content == Cell.CellContent.BlackHole)
                            {

                                ForegroundColor = ConsoleColor.Red;
                                Write(blackHole);

                            }
                            break;
                        case Cell.CellVisibility.Hidden:
                            Write(hidden);
                            break;
                        case Cell.CellVisibility.Flagged:
                            ForegroundColor = ConsoleColor.Yellow;
                            Write(flagged);
                            break;
                        default:
                            break;
                    }
                    ForegroundColor = currentColor;
                    Write(space);
                }
                Write(newLine);
            }
        }

        public static int DrawEnterFieldLength()
        {
            while (true)
            {
                WriteLine("Please enter playing field side size.");
                var userInput = ReadLine();
                var isValidInt = int.TryParse(userInput, out int i);
                if (!isValidInt || i < 1 || i > Game.maxAllowedLength)
                {
                    WriteLine("It must be an integer between 1 and {0} inclusively.\r\nThe value you entered (\'{1}\') is invalid", Game.maxAllowedLength, userInput);
                    continue;
                }

                WriteLine();
                return i;
            }
        }

        public static int DrawEnterBlackHolesCount(int boardLength)
        {
            var maxBlackHolesCount = boardLength * boardLength - 1;
            while (true)
            {
                WriteLine("Please enter number of black holes.");
                var userInput = ReadLine();
                var isValidInt = int.TryParse(userInput, out int i);
                if (!isValidInt || i < 1 || i > maxBlackHolesCount)
                {
                    WriteLine("It must be an integer between 1 and {0}.\r\nThe value you entered (\'{1}\') is invalid", maxBlackHolesCount, userInput);
                    continue;
                }

                WriteLine();
                return i;
            }
        }
        public int DrawEnterCoordinates(string axis)
        {
            while (true)
            {
                WriteLine("Please enter {0} coordinate.", axis);
                var userInput = ReadLine();
                var isValidInt = int.TryParse(userInput, out int i);
                if (!isValidInt || i < 0 || i >= Game.FieldLength)
                {
                    WriteLine("It must be an integer between 0 and {0}.\r\nThe value you entered (\'{1}\') is invalid", Game.FieldLength - 1, userInput);
                    continue;
                }
                WriteLine();
                return i;
            }
        }

        public static char DrawFlagOrOpen()
        {
            var validInput = new[] { 'f', 'u', 'o' };
            while (true)
            {
                WriteLine("Flag, unflag or open? (f/u/o)");
                var userInput = ReadLine();
                var firstChar = userInput.ToLower().ToCharArray().FirstOrDefault();

                if (!validInput.Any(x => x == firstChar))
                {
                    WriteLine("It must be \'f\', \'u\' or \'o\'.\r\nThe value you entered (\'{0}\') is invalid.", userInput);
                    continue;
                }
                WriteLine();
                return firstChar;
            }
        }
    }
}