using AutoMapper;
using JMusik.Dtos;
using JMusik.Models;

namespace JMusik.WebApi.Profiles
{
    public class JMusikProfile : Profile
    {
        public JMusikProfile()
        {
            CreateMap<Producto, ProductoDto>().ReverseMap();
            CreateMap<Perfil, PerfilDto>().ReverseMap();

            // Mapeamos de orden a ordenDto y asignamos a la propiedad string Usuario el valor del usuario mapeado desde la entidad x.Usuario.Username
            // Inversamente, se ignora, para no cargar toda la propiedad usuario desde la entidad
            CreateMap<Orden, OrdenDto>()
                .ForMember(u => u.Usuario, m => m.MapFrom(x => x.Usuario.Username))
                .ReverseMap()
                .ForMember(u => u.Usuario, m => m.Ignore());

            CreateMap<DetalleOrden, DetalleOrdenDto>()
                .ForMember(p => p.Producto, m => m.MapFrom(x => x.Producto.Nombre))
                .ReverseMap()
                .ForMember(p => p.Producto, m => m.Ignore());

            CreateMap<Usuario, UsuarioRegistroDto>()
                .ForMember(u => u.Perfil, p => p.MapFrom(m => m.Perfil.Nombre))
                .ReverseMap()
                .ForMember(u => u.Perfil, p => p.Ignore());

            CreateMap<Usuario, UsuarioActualizacionDto>().ReverseMap();

            CreateMap<Usuario, UsuarioListaDto>()
                .ForMember(u => u.Perfil, p => p.MapFrom(m => m.Perfil.Nombre))
                .ForMember(u => u.NombreCompleto, p => p.MapFrom(m => string.Format("{0} {1}", m.Nombre, m.Apellidos)))
                .ReverseMap();

            CreateMap<Usuario, LoginModelDto>().ReverseMap();

            CreateMap<Usuario, PerfilUsuarioDto>().ReverseMap();
        }
    }
}