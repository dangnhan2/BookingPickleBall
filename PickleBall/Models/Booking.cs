using PickleBall.Models.Enum;

namespace PickleBall.Models
{
    public class Booking
    {
        public Guid ID { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public Guid CourtID { get; set; }
        public Court Court { get; set; }
        public DateOnly BookingDate { get; set; }
        public int TotalAmount { get; set; }
        public Payment Payments { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public string QRCodeUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<BookingTimeSlots> BookingTimeSlots { get; set; } = new List<BookingTimeSlots>();
    }
}
