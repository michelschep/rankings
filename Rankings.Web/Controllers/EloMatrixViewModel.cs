using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Rankings.Web.Controllers
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class EloMatrixViewModel
    {
        public string Name { get; }
        public int Elo { get; }
        public int EloDiff { get; }
        public decimal Draw { get; }

        [DisplayName("1-0")]
        public decimal OneZeroWin { get; }
        
        [DisplayName("0-1")]
        public decimal OneZeroLost { get; }
        
        [DisplayName("2-0")]
        public decimal TwoZeroWin { get; }
        
        [DisplayName("0-2")]
        public decimal TwoZeroLost { get; }
        
        [DisplayName("3-0")]
        public decimal ThreeZeroWin { get; }
        
        [DisplayName("3-1")]
        public decimal ThreeOneWin { get; set; }

        [DisplayName("3-2")]
        public decimal ThreeTwoWin { get; set; }

        [DisplayName("0-3")]
        public decimal ThreeZeroLost { get; }

        [DisplayName("1-3")]
        public decimal ThreeOneLost { get; set; }
        [DisplayName("2-3")]
        public decimal ThreeTwoLost { get; set; }

        [DisplayName("1-2")]
        public decimal TwoOneLost { get; set; }
        [DisplayName("2-1")]
        public decimal TwoOneWin { get; set; }

        public EloMatrixViewModel(string name, int elo, int eloDiff, decimal draw, decimal oneZeroWin, decimal oneZeroLost, decimal twoZeroWin, decimal twoZeroLost, 
            decimal threeZeroWin, 
            decimal threeZeroLost)
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