namespace PickleBall.Dto.Request
{
    public class TimeSlotRequest
    {   
        public Guid PartnerId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
