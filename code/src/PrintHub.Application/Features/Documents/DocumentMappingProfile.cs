using AutoMapper;
using PrintHub.Application.Features.Documents.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Documents;

public class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        CreateMap<DocumentFile, DocumentDto>();
    }
}
