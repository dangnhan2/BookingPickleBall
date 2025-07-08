namespace PickleBall.Dto
{
    public class TimeSlotDto
    {
        public Guid ID { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
