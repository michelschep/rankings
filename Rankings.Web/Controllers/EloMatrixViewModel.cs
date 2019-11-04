using System.ComponentModel;

namespace Rankings.Web.Controllers
{
    public class EloMatrixViewModel
    {
        public string Name { get; }
        public int Elo { get; }
        public int EloDiff { get; }
        public int Draw { get; }

        [DisplayName("1-0")]
        public int OneZeroWin { get; }
        
        [DisplayName("0-1")]
        public int OneZeroLost { get; }
        
        [DisplayName("2-0")]
        public int TwoZeroWin { get; }
        
        [DisplayName("0-2")]
        public int TwoZeroLost { get; }
        
        [DisplayName("3-0")]
        public int ThreeZeroWin { get; }
        
        [DisplayName("0-3")]
        public int ThreeZeroLost { get; }

        public EloMatrixViewModel(string name, int elo, int eloDiff, int draw, int oneZeroWin, int oneZeroLost, int twoZeroWin, int twoZeroLost, int threeZeroWin, int threeZeroLost)
        {
            Name = name;
            Elo = elo;
            EloDiff = eloDiff;
            Draw = draw;
            OneZeroWin = oneZeroWin;
            OneZeroLost = oneZeroLost;
            TwoZeroWin = twoZeroWin;
            TwoZeroLost = twoZeroLost;
            ThreeZeroWin = threeZeroWin;
            ThreeZeroLost = threeZeroLost;
        }
    }
}