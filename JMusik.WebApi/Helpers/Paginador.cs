using System;
using System.Collections.Generic;

namespace JMusik.WebApi.Helpers
{
    public class Paginador<T> where T : class
    {
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public IEnumerable<T> Registros { get; set; }

        // Propiedades de solo lectura
        public int TotalPaginas
        {
            get
            {
                return (int)Math.Ceiling(TotalRegistros / (double)RegistrosPorPagina);
            }
        }

        public bool TienePaginaAnterior
        {
            get
            {
                return PaginaActual > 1;
            }
        }

        public bool TienePaginaSiguiente
        {
            get
            {
                return PaginaActual < TotalPaginas;
            }
        }

        public Paginador(int paginaActual, int registrosPorPagina, int totalRegistros, IEnumerable<T> registros)
        {
            PaginaActual = paginaActual;
            RegistrosPorPagina = registrosPorPagina;
            TotalRegistros = totalRegistros;
            Registros = registros;
        }
    }
}