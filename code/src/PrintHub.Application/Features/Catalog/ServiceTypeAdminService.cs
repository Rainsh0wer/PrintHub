using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Application.Specifications.ServiceTypes;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Catalog;

public class ServiceTypeAdminService : IServiceTypeAdminService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ServiceTypeAdminService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ServiceTypeAdminDto>>> ListAsync(CancellationToken ct = default)
    {
        var types = await _uow.Repository<ServiceType>().ListAsync(new AllServiceTypesSpecification(), ct);
        return Result.Success(_mapper.Map<IReadOnlyList<ServiceTypeAdminDto>>(types));
    }

    public async Task<Result<ServiceTypeAdminDto>> CreateAsync(CreateServiceTypeRequest request, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ServiceType>();
        if (await repo.AnyAsync(new ServiceTypeByCodeSpecification(request.Code), ct))
            return Result<ServiceTypeAdminDto>.Conflict("A service type with this code already exists.");

        var type = new ServiceType
        {
            Code = request.Code,
            Name = request.Name,
            ServiceGroup = request.ServiceGroup,
            PricingModel = request.PricingModel,
            UnitOfMeasure = request.UnitOfMeasure,
            RequiresFile = request.RequiresFile,
            Description = request.Description,
            DisplayOrder = request.DisplayOrder,
            IconUrl = request.IconUrl,
            IsActive = true
        };
        await repo.AddAsync(type, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<ServiceTypeAdminDto>(type));
    }

    public async Task<Result<ServiceTypeAdminDto>> UpdateAsync(int id, UpdateServiceTypeRequest request, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ServiceType>();
        var type = await repo.GetByIdAsync(id, ct);
        if (type is null)
            return Result<ServiceTypeAdminDto>.NotFound("Service type not found.");

        type.Name = request.Name;
        type.UnitOfMeasure = request.UnitOfMeasure;
        type.RequiresFile = request.RequiresFile;
        type.Description = request.Description;
        type.IsActive = request.IsActive;
        type.DisplayOrder = request.DisplayOrder;
        type.IconUrl = request.IconUrl;
        repo.Update(type);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<ServiceTypeAdminDto>(type));
    }

    public async Task<Result> DeactivateAsync(int id, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ServiceType>();
        var type = await repo.GetByIdAsync(id, ct);
        if (type is null)
            return Result.NotFound("Service type not found.");

        type.IsActive = false;
        repo.Update(type);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
