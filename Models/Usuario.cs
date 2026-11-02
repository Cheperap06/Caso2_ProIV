
namespace Caso2_ProIV.Models
{
    /// <summary>
    /// Representa a una persona autorizada
    /// para utilizar el sistema ElectroHogar.
    /// </summary>
    public class Usuario
    {
        // Identificador único del usuario.
        public int Id { get; set; }

        // Nombres del empleado o administrador.
        public string Nombres { get; set; }

        // Apellidos del empleado o administrador.
        public string Apellidos { get; set; }

        // Nombre completo calculado automáticamente.
        public string NombreCompleto
        {
            get { return Nombres + " " + Apellidos; }
        }

        // Nombre utilizado para iniciar sesión.
        public string NombreUsuario { get; set; }

        // Contraseña almacenada como hash.
        public string HashContrasena { get; set; }

        // Rol que determina sus permisos.
        public RolUsuario Rol { get; set; }

        // Indica si la cuenta está habilitada.
        public bool Activo { get; set; }

        /// <summary>
        /// Constructor vacío.
        /// </summary>
        public Usuario()
        {
            Activo = true;
        }

        /// <summary>
        /// Constructor para crear un usuario
        /// con sus datos principales.
        /// </summary>
        public Usuario(
            int id,
            string nombres,
            string apellidos,
            string nombreUsuario,
            string hashContrasena,
            RolUsuario rol)
        {
            Id = id;
            Nombres = nombres;
            Apellidos = apellidos;
            NombreUsuario = nombreUsuario;
            HashContrasena = hashContrasena;
            Rol = rol;
            Activo = true;
        }

        /// <summary>
        /// Devuelve información básica del usuario.
        /// </summary>
        public override string ToString()
        {
            return NombreCompleto + " (" + Rol + ")";
        }
    }
}
