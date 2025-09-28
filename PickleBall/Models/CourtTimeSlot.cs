namespace PickleBall.Models
{
    public class CourtTimeSlot
    {
        public Guid ID { get; set; }
        public Guid CourtID { get; set; }
        public Court Court { get; set; }
        public Guid TimeSlotID { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public ICollection<BookingTimeSlots> BookingTimeSlots { get; set; } = new List<BookingTimeSlots>();
    }
}
