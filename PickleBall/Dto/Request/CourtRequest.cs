using PickleBall.Models.Enum;

namespace PickleBall.Dto.Request
{
    public class CourtRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public CourtStatus CourtStatus { get; set; }
    }
}
