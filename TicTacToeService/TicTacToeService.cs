namespace TicTacToeService
{
    public static class TicTacToeService
    {
        private static Action<int>? makeMoveAction;
        private static int i = 0;

        public static void RegisterMakeMoveAction(Action<int> action)
        {
            makeMoveAction = action;
            i = 2;
        }

        public static void MakeMove(int field)
        {
            makeMoveAction?.Invoke(field);
        }
    }
}