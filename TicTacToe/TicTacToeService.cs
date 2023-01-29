using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace TicTacToe
{
    public static class TicTacToeService
    {
        private static Action<int>? makeMoveAction;

        public static void RegisterMakeMoveAction(Action<int> action)
        {
            makeMoveAction = action;
        }

        public static void StartLoop(int move)
        {
            while (true)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TicTacToe\\Moves\\" + move;
                if (!File.Exists(path))
                {
                    Thread.Sleep(100);
                    continue;
                }
                
                makeMoveAction?.Invoke(int.Parse(File.ReadAllText(path)));
            }
        }

        public static void InitMoves()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TicTacToe\\Moves";

            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
    }
}