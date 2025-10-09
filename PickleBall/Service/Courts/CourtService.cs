using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;
using PickleBall.Extension;
using PickleBall.Models;
using PickleBall.Models.Enum;
using PickleBall.Service.Storage;
using PickleBall.UnitOfWork;
using PickleBall.Validation;
using System.Reflection.Metadata.Ecma335;

namespace PickleBall.Service.Courts
{
    public class CourtService : ICourtService
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string[] allowedExtension = { ".jpg", ".png", ".jpeg", };
        private const string folder = "Court";
        private readonly BookingContext _context;

        public CourtService(IUnitOfWorks unitOfWork, ICloudinaryService cloudinaryService, BookingContext context) { 
          _unitOfWork = unitOfWork;
          _cloudinaryService = cloudinaryService;
          _context = context;
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
                PartnerId = court.PartnerId,
                Name = court.Name,
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

            isExistCourt.PartnerId = court.PartnerId;
            isExistCourt.Name = court.Name;
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
                    ID = Guid.NewGuid(),
                    CourtID = isExistCourt.ID,
                    TimeSlotID = slotId
                };

                isExistCourt.CourtTimeSlots.Add(newMapping);
            }

            _unitOfWork.Court.Update(isExistCourt);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Cập nhật thành công", StatusCodes.Status200OK);
        }

        public async Task<DataReponse<CourtDto>> GetAllByPartner(Guid id, CourtParams court)
        {
            var courts = _unitOfWork.Court.GetAllByPartner(id);

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

        public async Task<Result<CourtDto>> GetById(Guid id)
        {
            var court = await _unitOfWork.Court.GetById(id);

            if(court == null)
            {
                return Result<CourtDto>.Fail("Không tìm thấy sân", StatusCodes.Status200OK);
            }

            var courtToDto = new CourtDto
            {
                ID = court.ID,
                Name = court.Name,
                Location = court.Location,
                PricePerHour = court.PricePerHour,
                ImageUrl = court.ImageUrl,
                CourtStatus = court.CourtStatus,
                Created = court.Created,
                TimeSlotIDs = court.CourtTimeSlots.Select(s => new TimeSlotDto
                {
                  ID = s.ID,
                  StartTime = s.TimeSlot.StartTime,
                  EndTime = s.TimeSlot.EndTime
                }).ToList()
            };

            return Result<CourtDto>.Ok(courtToDto, StatusCodes.Status200OK);
        }

        public async Task<IEnumerable<CourtDto>> GetAllInSpecificDate(Guid id, DateOnly date)
        {
            return await _context.Courts
                 .Where(c => c.PartnerId == id)
                   .Select(c => new CourtDto
                   {
                      ID = c.ID,
                      Name = c.Name,
                      ImageUrl = c.ImageUrl,
                      PricePerHour = c.PricePerHour,
                      CourtStatus =c.CourtStatus,
                      Location = c.Location,
                      TimeSlotIDs = c.CourtTimeSlots
                   .Select(ts => new TimeSlotDto
                   {
                      ID = ts.ID,
                      StartTime = ts.TimeSlot.StartTime,
                      EndTime = ts.TimeSlot.EndTime,
                      Status = ts.BookingTimeSlots
                   .Where(bt => bt.Booking.BookingDate == date && bt.Booking.ExpriedAt > DateTime.UtcNow)
                   .Select(bt => (BookingStatus?)bt.Booking.BookingStatus)
    .              FirstOrDefault() ?? BookingStatus.Free
                    })
                    .OrderBy(s => s.StartTime)
                     .ToList()
                    }).ToListAsync();

        }      

        public async Task<IEnumerable<UserDto>> GetAllPartnerInfo()
        {
            var partners = _unitOfWork.User.Get();

            var partnersToDto = await partners.Where(p => !p.IsAdmin && p.IsApproved).Select(p => new UserDto
            {   
                ID = p.Id,
                BussinessName = p.BussinessName,
                Address = p.Address
            }).ToListAsync();

            return partnersToDto;
        }
    }
}

//b3f0c7d0-c7f1-474a-8bd6-66883787c0d7
//2025-09-27