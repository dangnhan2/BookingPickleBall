using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;
using PickleBall.Extension;
using PickleBall.Models;
using PickleBall.Service.Storage;
using PickleBall.UnitOfWork;
using PickleBall.Validation;

namespace PickleBall.Service.Courts
{
    public class CourtService : ICourtService
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string[] allowedExtension = { ".jpg", ".png", ".jpeg", };
        private const string folder = "Court";
        public CourtService(IUnitOfWorks unitOfWork, ICloudinaryService cloudinaryService) { 
          _unitOfWork = unitOfWork;
          _cloudinaryService = cloudinaryService;
        }

        public async Task<Result<string>> Add(CourtRequest court)
        {
            var validator = new CourtRequestValidator();

            var result = validator.Validate(court);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var courts = _unitOfWork.Court.Get();

            if(await courts.AnyAsync(c => c.Name.ToLower() == court.Name.ToLower()))
            {
                return Result<string>.Fail("Sân đã tồn tại", StatusCodes.Status400BadRequest);
            }

            var newCourt = new Court
            {
                ID = Guid.NewGuid(),
                Name = court.Name,
                Description = court.Description,
                Location = court.Location,
                PricePerHour = court.PricePerHour,
                CourtStatus = court.CourtStatus,
                Created = DateTime.UtcNow,
            };

            foreach(var slot in court.TimeSlotIDs)
            {
                var courtTimeSlot = new CourtTimeSlot
                {   
                    ID = Guid.NewGuid(),
                    CourtID = newCourt.ID,
                    TimeSlotID = slot
                };

                newCourt.CourtTimeSlots.Add(courtTimeSlot);
            }

            if (court.ImageUrl == null || court.ImageUrl.Length == 0)
            {
                return Result<string>.Fail("File phải được upload", StatusCodes.Status400BadRequest);
            }

            var imageUrl = await _cloudinaryService.Upload(court.ImageUrl, allowedExtension, folder);

            newCourt.ImageUrl = imageUrl;

            await _unitOfWork.Court.CreateAsync(newCourt);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Thêm mới thành công", StatusCodes.Status201Created);
        }

        public async Task<Result<string>> Delete(Guid id)
        {
            var isExistCourt = await _unitOfWork.Court.GetById(id);

            if(isExistCourt == null)
            {
                return Result<string>.Fail("Không tìm thấy sân", StatusCodes.Status404NotFound);
            }

            _unitOfWork.Court.Delete(isExistCourt);
            await _cloudinaryService.Delete(isExistCourt.ImageUrl);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Xóa thành công", StatusCodes.Status200OK);
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

        public async Task<Result<CourtForIdDto>> GetById(Guid id)
        {
          
            var isExistCourt = await _unitOfWork.Court.GetById(id);

            if( isExistCourt == null)
            {
                return Result<CourtForIdDto>.Fail("Không tìm thấy sân", StatusCodes.Status404NotFound);
            }
           
            var courtToDto = new CourtForIdDto
            {
                ID = isExistCourt.ID,
                Name = isExistCourt.Name,
                Description = isExistCourt.Description,
                Location = isExistCourt.Location,
                PricePerHour = isExistCourt.PricePerHour,
                ImageUrl = isExistCourt.ImageUrl,
                CourtStatus = isExistCourt.CourtStatus,
                Created = isExistCourt.Created,
                TimeSlotIDs = isExistCourt.CourtTimeSlots.Select(tl => new TimeSlotDto
                {
                    ID = tl.TimeSlotID,
                    StartTime = tl.TimeSlot.StartTime,
                    EndTime = tl.TimeSlot.EndTime,
                }).ToList()
            };

            return Result<CourtForIdDto>.Ok(courtToDto, StatusCodes.Status200OK);
        }

        public async Task<Result<string>> Update(Guid id, CourtRequest court)
        {
            var validator = new CourtRequestValidator();

            var result = validator.Validate(court);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var isExistCourt = await _unitOfWork.Court.GetById(id);

            if(isExistCourt == null)
            {
                return Result<string>.Fail("Không tìm thấy sân", StatusCodes.Status404NotFound);
            }

            var courts = _unitOfWork.Court.Get();

            if (await courts.AnyAsync(c => c.Name.ToLower() == court.Name.ToLower() && c.ID != id))
            {
                return Result<string>.Fail("Sân đã tồn tại", StatusCodes.Status400BadRequest);
            }

            if (court.ImageUrl != null)
            {
                await _cloudinaryService.Delete(isExistCourt.ImageUrl);

                var imageUrl = await _cloudinaryService.Upload(court.ImageUrl, allowedExtension, folder);

                isExistCourt.ImageUrl = imageUrl;
            }

            isExistCourt.Name = court.Name;
            isExistCourt.Description = court.Description;
            isExistCourt.Location = court.Location;
            isExistCourt.PricePerHour = court.PricePerHour;
            isExistCourt.CourtStatus = court.CourtStatus;

            var uniqueIds = court.TimeSlotIDs.Distinct();

            var oldMappings = await _unitOfWork.CourtTimeSlot.FindAsyncByCourtId(isExistCourt.ID);
            _unitOfWork.CourtTimeSlot.RemoveRange(oldMappings);

            foreach (var slotId in uniqueIds)
            {
                var newMapping = new CourtTimeSlot
                {
                    CourtID = isExistCourt.ID,
                    TimeSlotID = slotId
                };

                isExistCourt.CourtTimeSlots.Add(newMapping);
            }

            _unitOfWork.Court.Update(isExistCourt);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Cập nhật thành công", StatusCodes.Status200OK);
        }
    }
}
