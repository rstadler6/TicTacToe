using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        //private string[,] board = new string[3, 3];
        private readonly Button[,] board;
        private readonly HttpClient client = new();
        private bool player1Turn = true;
        private int aiMove = 1;

        public MainWindow()
        {
            InitializeComponent();
            TicTacToeService.RegisterMakeMoveAction(MakeMove);

            board = new [,]
            {
                {button1, button2, button3},
                {button4, button5, button6},
                {button7, button8, button9}
            };

            ResetGame();
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

            button.Content = "O";

            SendToAI();
            FinishTurn();
        }

        private void FinishTurn()
        {
            player1Turn = !player1Turn;

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

            aiMove = 1;
            TicTacToeService.InitMoves();
            StartBackend();
            
            var startSelection = new StartSelection();
            if (startSelection.ShowDialog() == false)
            {
                SendStartingPlayer("computer");
                player1Turn = false;
            }
            else
            {
                SendStartingPlayer("player");
                player1Turn = true;
            }
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

        private void StartBackend()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8080/process-definition/key/ai/start");
            client.Send(request);
        }

        private async void SendStartingPlayer(string player)
        {
            var getTasksRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8080/engine-rest/task?processDefinitionKey=ai");
            var content = (await client.SendAsync(getTasksRequest)).Content;
            var task = JsonConvert.DeserializeObject<TaskResponse>(await content.ReadAsStringAsync());

            var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:8080/engine-rest/task/{task.id}/resolve");
            var json =
                $@"
{{
  'variables': {{
            'beginner': {{
                'value': {player}
            }}
        }},
        'withVariablesInReturn': false
}}
";
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.SendAsync(request);
        }

        private async void SendToAI()
        {
            var getTasksRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8080/engine-rest/task?processDefinitionKey=ai");
            var content = (await client.SendAsync(getTasksRequest)).Content;
            var task = JsonConvert.DeserializeObject<TaskResponse>(await content.ReadAsStringAsync());

            var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:8080/engine-rest/task/{task.id}/resolve");
            var json =
                $@"
{{
  'variables': {{
            'fields': {{
                'value': {ConvertGameState().ToCharArray()}
            }}
        }},
        'withVariablesInReturn': false
}}
";
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.SendAsync(request);

            Task.Run(() => TicTacToeService.StartLoop(aiMove++));
        }

        private void MakeMove(int field)
        {
            Dispatcher.Invoke(() =>
            {
                board[(field - 1) / 3, (field - 1) % 3].Content = "X";
                FinishTurn();
            });
        }

    }

    class FieldStateTask
    {
        public char[] fields { get; set; }
    }

    class TaskResponse
    {
        public string id { get; set; }
    }
}