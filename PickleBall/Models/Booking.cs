using PickleBall.Enum;

namespace PickleBall.Models
{
    public class Booking
    {
        public Guid ID { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public Guid CourtID { get; set; }
        public Court Courts { get; set; }
        public Guid TimeSlotID { get; set; }
        public TimeSlot TimeSlots { get; set; }
        public DateOnly BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public Payment Payments { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public string QRCodeUrl { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
