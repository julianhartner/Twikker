using System;
using System.Collections.Generic;

namespace Twikker.Data.Models
{
    public class TwikkerPost : IComparable<TwikkerPost>
    {
        public TwikkerPost()
        {
            Comments = new List<TwikkerComment>();
            Likes = new List<PostLike>();
        }

        public int ID { get; set; }

        public string Content { get; set; }

        public DateTime PostDate { get; set; }

        public ApplicationUser Owner { get; set; }

        public ICollection<PostLike> Likes { get; set; }

        public ICollection<TwikkerComment> Comments { get; set; }

        public int CompareTo(TwikkerPost other)
        {
            return other.PostDate.CompareTo(PostDate);
        }
    }
}