using PickleBall.Models.Enum;

namespace PickleBall.Dto.QueryParams
{
    public class CourtParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Name { get; set; }
        public CourtStatus? Status { get; set; } 

    }
}
