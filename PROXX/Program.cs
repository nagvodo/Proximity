using System;
using static System.Console;
using static PROXX.Domain.Game;

namespace PROXX
{
    public class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {

                var length = View.DrawEnterFieldLength();
                var blackHolesCount = View.DrawEnterBlackHolesCount(length);
                var view = new View(length, blackHolesCount);
                var state = view.Game.State;

                while (true)
                {
                    view.DrawPlayingField();

                    if (state != GameState.InProgress)
                        break;

                    int x;
                    x = view.DrawEnterCoordinates(nameof(x));

                    int y;
                    y = view.DrawEnterCoordinates(nameof(y));

                    var coordinates = (x, y);

                    char userInput = View.DrawFlagOrOpen();
                    switch (userInput)
                    {
                        case 'f':
                            view.Game.Flag(coordinates);
                            continue;
                        case 'u':
                            view.Game.UnFlag(coordinates);
                            continue;
                        default:
                            break;

                    }

                    state = view.Game.OpenCell(coordinates);
                }

                var currentColor = ForegroundColor;
                if (state == GameState.Lost)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Sorry! You lost.");
                }
                else if (state == GameState.Won)
                {
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("Congratulations! You won!");
                }

                ForegroundColor = currentColor;
                WriteLine("Press any key to play one more time!");
                ReadLine();
            }
        }
    }
}