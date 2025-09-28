using PickleBall.Models.Enum;

namespace PickleBall.Models
{
    public class Court
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public string ImageUrl { get; set; }
        public CourtStatus CourtStatus { get; set; }
        public DateTime Created { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<CourtTimeSlot> CourtTimeSlots { get; set; } = new List<CourtTimeSlot>();
    }
}
