using PickleBall.Enum;

namespace PickleBall.Models
{
    public class Payment
    {
        public Guid ID { get; set; }
        public Guid BookingID { get; set; }
        public Booking Bookings { get; set; }
        public string MethodPayment { get; set; }
        public string TransactionID { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaidAt { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public bool IsDeleted { get; set; }
    }
}
