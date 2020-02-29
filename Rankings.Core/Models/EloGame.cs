using Rankings.Core.Entities;

namespace Rankings.Core.Models
{
    public class EloGame
    {
        public Game Game { get; }
        public decimal EloPlayer1 { get; }
        public decimal EloPlayer2 { get; }
        public decimal Player1Delta { get; }
        public decimal Player2Delta { get; }

        public EloGame()
        {

        }

        public EloGame(Game game, decimal eloPlayer1, decimal eloPlayer2, decimal player1Delta, decimal player2Delta)
        {
            Game = game;
            EloPlayer1 = eloPlayer1;
            EloPlayer2 = eloPlayer2;
            Player1Delta = player1Delta;
            Player2Delta = player2Delta;
        }
    }
}