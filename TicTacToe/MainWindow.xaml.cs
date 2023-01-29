using System;
using System.CodeDom;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TicTacToeService;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        //private string[,] board = new string[3, 3];
        private readonly Button[,] board;
        private readonly HttpClient client = new();
        private bool player1Turn = true;

        public MainWindow()
        {
            InitializeComponent();
            TicTacToeService.TicTacToeService.RegisterMakeMoveAction(MakeMove);

            board = new [,]
            {
                {button1, button2, button3},
                {button4, button5, button6},
                {button7, button8, button9}
            };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j].Content = "";
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!player1Turn)
                return;

            Button button = (Button)sender;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);
            
            if ((string)button.Content != "")
            {
                MessageBox.Show("This cell is already occupied!");
                return;
            }

            button.Content = "X";
            player1Turn = false;

            if (IsWin())
            {
                MessageBox.Show("Player " + (player1Turn ? "2" : "1") + " wins!");
                ResetGame();
            }
            else if (IsDraw())
            {
                MessageBox.Show("It's a draw!");
                ResetGame();
            }

            SendToAI();
        }

        private bool IsWin()
        {
            // Check rows
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0].Content == board[i, 1].Content && board[i, 1].Content == board[i, 2].Content && (string)board[i, 0].Content != "")
                {
                    return true;
                }
            }

            // Check columns
            for (int i = 0; i < 3; i++)
            {
                if (board[0, i].Content == board[1, i].Content && board[1, i].Content == board[2, i].Content && (string)board[0, i].Content != "")
                {
                    return true;
                }
            }

            // Check diagonals
            if (board[0, 0].Content == board[1, 1].Content && board[1, 1].Content == board[2, 2].Content && (string)board[0, 0].Content != "")
            {
                return true;
            }

            if (board[0, 2].Content == board[1, 1].Content && board[1, 1].Content == board[2, 0].Content && (string)board[0, 2].Content != "")
            {
                return true;
            }

            return false;
        }

        private bool IsDraw()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if ((string)board[i, j].Content == "")
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ResetGame()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j].Content = "";
                }
            }

            player1Turn = true;
        }

        private string ConvertGameState()
        {
            var gameState = string.Empty;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    gameState += $"{board[i, j].Content},";
                }
            }

            gameState.TrimEnd(',');
            return gameState;
        }

        private void SetGameState(string gameState)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j].Content = gameState[i * 3 + j];
                }
            }

            player1Turn = true;
        }

        private void SendToAI()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://google.ch");
            request.Content = new StringContent(ConvertGameState());
            client.Send(request);
        }

        private void MakeMove(int field)
        {
            board[(field - 1) / 3, (field - 1) % 3].Content = "X";
            player1Turn = true;
        }
    }
}