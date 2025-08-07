using PickleBall.Models.Enum;

namespace PickleBall.Dto
{
    public class CourtForIdDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public string ImageUrl { get; set; }
        public CourtStatus CourtStatus { get; set; }
        public DateTime Created { get; set; }
        public List<TimeSlotDto> TimeSlotIDs { get; set; } = new List<TimeSlotDto>();
    }
}
