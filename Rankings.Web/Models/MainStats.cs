using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Rankings.Web.Controllers;

namespace Rankings.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class MainStats
    {
        public IEnumerable<Summary> HallOfFame { get; set; }
        public IEnumerable<Summary> RunningBattles { get; set; }
        public IEnumerable<Summary> Eternal { get; set; }
    }
}