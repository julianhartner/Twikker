using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Twikker.Web.Models.HomeViewModels
{
    public class IndexViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string NewPostContent { get; set; }
        
    }
}
