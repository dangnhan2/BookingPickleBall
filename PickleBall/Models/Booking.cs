using PickleBall.Models.Enum;

namespace PickleBall.Models
{
    public class Booking
    {
        public Guid ID { get; set; }
        public Guid CourtID { get; set; }
        public Court Court { get; set; }
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
        public DateOnly BookingDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpriedAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
        public string TransactionId { get; set; }
        public int TotalAmount { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<BookingTimeSlots> BookingTimeSlots { get; set; } = new List<BookingTimeSlots>();
    }
}
