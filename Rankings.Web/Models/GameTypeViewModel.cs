using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class GameTypeViewModel
    {
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public GameTypeViewModel()
        {
        }
    }
}