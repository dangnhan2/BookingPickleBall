namespace PickleBall.Models
{
    public class TimeSlot
    {
        public Guid ID { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
