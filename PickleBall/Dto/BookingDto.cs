using PickleBall.Models.Enum;

namespace PickleBall.Dto
{
    public class BookingDto
    {   
        public Guid ID { get; set; }
        public string Customer { get; set; }
        public string Phone { get; set; }
        public string Court { get; set; }
        public DateOnly BookingDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public int TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; } 
        public List<TimeSlotDto> TimeSlots { get; set; } 
       
    }
}
