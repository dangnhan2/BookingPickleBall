namespace PickleBall.Models
{
    public class BookingTimeSlots
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; }
        public Guid CourtTimeSlotId { get; set; }
        public CourtTimeSlot CourtTimeSlots { get; set; }
    }
}
