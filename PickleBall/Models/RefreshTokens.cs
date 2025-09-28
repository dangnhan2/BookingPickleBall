namespace PickleBall.Models
{
    public class RefreshTokens
    {
        public Guid ID { get; set; }
        public string RefreshToken { get; set; }
        public bool IsLocked { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RevokedAt { get; set; }
        public Guid UserID { get; set; }
        public Partner? User { get; set; }
    }
}
