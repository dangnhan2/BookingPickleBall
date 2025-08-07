namespace PickleBall.Models
{
    public class BookingTimeSlots
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; }
        public Guid TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; }
        
    }
}
