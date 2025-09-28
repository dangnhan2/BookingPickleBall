using PickleBall.Models.Enum;

namespace PickleBall.Dto.Request
{
    public class CourtRequest
    {   
        public Guid PartnerId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public CourtStatus CourtStatus { get; set; }
        public List<Guid> TimeSlotIDs { get; set; } = new List<Guid>();
    }
}
