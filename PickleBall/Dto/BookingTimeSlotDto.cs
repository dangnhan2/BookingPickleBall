namespace PickleBall.Dto
{
    public class BookingTimeSlotDto
    {
        public Guid Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
