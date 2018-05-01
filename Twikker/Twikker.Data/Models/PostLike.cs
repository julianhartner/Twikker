namespace Twikker.Data.Models
{
    public class PostLike
    {
        public int ID { get; set; }

        public ApplicationUser User { get; set; }

        public TwikkerPost Post { get; set; }
    }
}