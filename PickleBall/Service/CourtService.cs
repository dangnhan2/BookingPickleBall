using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Extension;
using PickleBall.Models;
using PickleBall.QueryParams;
using PickleBall.Service.SoftService;
using PickleBall.UnitOfWork;

namespace PickleBall.Service
{
    public class CourtService : ICourtService
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string[] allowedExtension = { ".jpg", ".png", ".jpeg", };
        private readonly string folder = "Court";
        public CourtService(IUnitOfWorks unitOfWork, ICloudinaryService cloudinaryService) { 
          _unitOfWork = unitOfWork;
          _cloudinaryService = cloudinaryService;
        }

        public async Task Add(CourtRequest court)
        {  
            var courts = _unitOfWork.Court.Get();

            if(await courts.AnyAsync(c => c.Name.ToLower() == court.Name.ToLower()))
            {
                throw new ArgumentException("Sân đã tồn tại");
            }

            var newCourt = new Court
            {
                Name = court.Name,
                Description = court.Description,
                Location = court.Location,
                PricePerHour = court.PricePerHour,
                CourtStatus = court.CourtStatus,
                IsDeleted = false,
                Created = DateTime.UtcNow,
            };

            if (court.ImageUrl == null || court.ImageUrl.Length == 0)
            {
                throw new ArgumentException("File phải được upload");
            }

            var imageUrl = await _cloudinaryService.Upload(court.ImageUrl, allowedExtension, folder);

            newCourt.ImageUrl = imageUrl;

            await _unitOfWork.Court.CreateAsync(newCourt);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(Guid id)
        {
            var isExistCourt = await _unitOfWork.Court.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy sân");

            isExistCourt.IsDeleted = true;
            _unitOfWork.Court.Update(isExistCourt);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<DataReponse<CourtDto>> GetAll(CourtParams court)
        {
            var courts = _unitOfWork.Court.Get();

            if (!string.IsNullOrEmpty(court.Name))
            {
                courts = courts.Where(c => c.Name.ToLower().Contains(court.Name.ToLower()));
            }

            if (court.Status.HasValue)
            {
                courts = courts.Where(c => c.CourtStatus == court.Status);
            }

            var courtsToDto = courts.Select(c => new CourtDto
            {
                ID = c.ID,
                Name = c.Name,
                Description = c.Description,
                Location = c.Location,
                PricePerHour = c.PricePerHour,
                ImageUrl = c.ImageUrl,
                CourtStatus = c.CourtStatus,
                Created = c.Created,
            }).Paging(court.Page, court.PageSize);

            return new DataReponse<CourtDto>
            {
                Page = court.Page,
                PageSize = court.PageSize,
                Total = courts.Count(),
                Data = await courtsToDto.ToListAsync()
            };
        }

        public async Task<CourtDto> GetById(Guid id)
        {
            var isExistCourt = await _unitOfWork.Court.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy sân");
           
            var courtToDto = new CourtDto
            {
                ID = isExistCourt.ID,
                Name = isExistCourt.Name,
                Description = isExistCourt.Description,
                Location = isExistCourt.Location,
                PricePerHour = isExistCourt.PricePerHour,
                ImageUrl = isExistCourt.ImageUrl,
                CourtStatus = isExistCourt.CourtStatus,
                Created = isExistCourt.Created,
            };

            return courtToDto;
        }

        public async Task Update(Guid id, CourtRequest court)
        {   
            var isExistCourt = await _unitOfWork.Court.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy sân");

            var courts = _unitOfWork.Court.Get();

            if (await courts.AnyAsync(c => c.Name.ToLower() == court.Name.ToLower() && c.ID != id))
            {
                throw new ArgumentException("Sân đã tồn tại");
            }

            if (court.ImageUrl != null)
            {
                await _cloudinaryService.Delete(isExistCourt.ImageUrl);

                var imageUrl = await _cloudinaryService.Upload(court.ImageUrl, allowedExtension, folder);

                isExistCourt.Name = court.Name;
                isExistCourt.Description = court.Description;
                isExistCourt.Location = court.Location;
                isExistCourt.PricePerHour = court.PricePerHour;
                isExistCourt.ImageUrl = imageUrl;
                isExistCourt.CourtStatus = court.CourtStatus;

                _unitOfWork.Court.Update(isExistCourt);
                await _unitOfWork.CompleteAsync();
            }

            isExistCourt.Name = court.Name;
            isExistCourt.Description = court.Description;
            isExistCourt.Location = court.Location;
            isExistCourt.PricePerHour = court.PricePerHour;
            isExistCourt.CourtStatus = court.CourtStatus;

            _unitOfWork.Court.Update(isExistCourt);
            await _unitOfWork.CompleteAsync();
        }
    }
}
