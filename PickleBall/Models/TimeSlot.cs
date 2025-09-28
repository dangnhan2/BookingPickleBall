namespace PickleBall.Models
{
    public class TimeSlot
    {
        public Guid ID { get; set; }
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public List<CourtTimeSlot>? CourtTimeSlots { get; set; } 
        public List<BookingTimeSlots>? BookingTimeSlots { get; set; } 

    }
}
