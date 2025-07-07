using PickleBall.Enum;

namespace PickleBall.Dto.Request
{
    public class CourtRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public string ImageUrl { get; set; }
        public CourtStatus CourtStatus { get; set; }
        public DateTime Created { get; set; }
    }
}
