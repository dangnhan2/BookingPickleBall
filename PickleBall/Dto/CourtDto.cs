using PickleBall.Enum;
using PickleBall.Service;
using PickleBall.UnitOfWork;

namespace PickleBall.Dto
{
    public class CourtDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public string ImageUrl { get; set; }
        public CourtStatus CourtStatus { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
    }
}
