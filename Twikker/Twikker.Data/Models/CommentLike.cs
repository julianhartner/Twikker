namespace Twikker.Data.Models
{
    public class CommentLike
    {
        public int ID { get; set; }

        public ApplicationUser User { get; set; }

        public TwikkerComment Comment { get; set; }
    }
}