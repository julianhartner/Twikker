using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twikker.Data.Models
{
    public class TwikkerComment
    {
        public TwikkerComment()
        {
            Likes = new List<CommentLike>();
        }

        public int ID { get; set; }

        public string Content { get; set; }

        public DateTime PostDate { get; set; }

        public ApplicationUser Owner { get; set; }

        public ICollection<CommentLike> Likes { get; set; }
    }
}