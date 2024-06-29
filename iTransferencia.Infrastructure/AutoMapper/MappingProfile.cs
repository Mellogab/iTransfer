using AutoMapper;
using iTransferencia.Core.Entities;
using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.UseCases.Bacen.NotifyBacen;

namespace iTransferencia.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Transfer, MoveTransfer>()
                .ForMember(dest => dest.valor, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.idCliente, opt => opt.MapFrom(src => src.IdClient))
                .ForPath(dest => dest.conta.idOrigem, opt => opt.MapFrom(src => src.IdSourceAccount))
                .ForPath(dest => dest.conta.idDestino, opt => opt.MapFrom(src => src.IdDestinationAccount))
                .ReverseMap();

            CreateMap<NotifyBacenUseCaseInput, MoveTransfer>()
                .ForMember(dest => dest.valor, opt => opt.MapFrom(src => src.Value))
                .ForPath(dest => dest.conta.idOrigem, opt => opt.MapFrom(src => src.IdSourceAccount))
                .ForPath(dest => dest.conta.idDestino, opt => opt.MapFrom(src => src.IdDestinationAccount))
                .ReverseMap();  
        }
    }
}