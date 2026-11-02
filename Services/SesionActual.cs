
using Caso2_ProIV.Models;

namespace Caso2_ProIV.Services
{
    /// <summary>
    /// Administra la sesión del usuario
    /// que está utilizando el sistema.
    /// </summary>
    public static class SesionActual
    {
        // Usuario que tiene la sesión iniciada.
        public static Usuario Usuario { get; private set; }

        /// <summary>
        /// Guarda el usuario que inició sesión.
        /// </summary>
        public static void IniciarSesion(Usuario usuario)
        {
            Usuario = usuario;
        }

        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        public static void CerrarSesion()
        {
            Usuario = null;
        }

        /// <summary>
        /// Indica si existe una sesión iniciada.
        /// </summary>
        public static bool HaySesionActiva()
        {
            return Usuario != null;
        }

        /// <summary>
        /// Verifica si el usuario actual es administrador.
        /// </summary>
        public static bool EsAdministrador()
        {
            return Usuario != null &&
                   Usuario.Rol == RolUsuario.Administrador;
        }
    }
}
