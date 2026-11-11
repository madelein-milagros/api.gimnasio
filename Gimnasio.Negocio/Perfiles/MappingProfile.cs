using AutoMapper;
using Gimnasio.Entidades;
using Gimnasio.Entidades.DTOs;

namespace Gimnasio.Negocio.Perfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeos de Membresia
        CreateMap<Membresia, MembresiaDto>()
            .ForMember(dest => dest.TotalSocios,
                opt => opt.MapFrom(src => src.Socios.Count));

        CreateMap<MembresiaCrearDto, Membresia>();

        CreateMap<MembresiaActualizarDto, Membresia>();

        // Mapeos de Socio
        CreateMap<Socio, SocioDto>()
            .ForMember(dest => dest.Membresia,
                opt => opt.MapFrom(src => src.Membresia.Nombre))
            .ForMember(dest => dest.PrecioMensual,
                opt => opt.MapFrom(src => src.Membresia.PrecioMensual));

        CreateMap<SocioCrearDto, Socio>();

        CreateMap<SocioActualizarDto, Socio>();
    }
}
