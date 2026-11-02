
using System;
using System.Security.Cryptography;
using System.Linq;
using Caso2_ProIV.Models;
using Caso2_ProIV.Helpers;

namespace Caso2_ProIV.Services
{
    /// <summary>
    /// Administra los usuarios y su autenticación
    /// dentro del sistema ElectroHogar.
    /// </summary>
    public class UsuarioService
    {

        /// <summary>
        /// Busca un usuario por su nombre de usuario.
        /// Devuelve null si no existe.
        /// </summary>
        public Usuario ConsultarUsuario(string nombreUsuario)
        {
            // Verifica que el nombre de usuario no esté vacío.
            if (!Validador.CampoObligatorio(nombreUsuario))
            {
                return null;
            }

            // Busca el usuario sin distinguir mayúsculas y minúsculas.
            return AlmacenDatos.Instancia.Usuarios
                .FirstOrDefault(u =>
                    string.Equals(
                        u.NombreUsuario,
                        nombreUsuario.Trim(),
                        StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Genera un hash seguro a partir de una contraseña.
        /// Utiliza PBKDF2 con SHA-256 y una sal aleatoria.
        /// </summary>
        private string GenerarHashContrasena(string contrasena)
        {
            // Genera una sal aleatoria de 16 bytes.
            byte[] sal = new byte[16];

            using (RandomNumberGenerator generador =
                   RandomNumberGenerator.Create())
            {
                generador.GetBytes(sal);
            }

            // Deriva un hash de 32 bytes a partir de la contraseña.
            byte[] hash;

            using (Rfc2898DeriveBytes pbkdf2 =
                   new Rfc2898DeriveBytes(
                       contrasena,
                       sal,
                       600000,
                       HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(32);
            }

            // Almacena tanto la sal como el hash.
            return Convert.ToBase64String(sal) + ":" +
                   Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifica si una contraseña coincide con el hash almacenado.
        /// </summary>
        private bool VerificarContrasena(
            string contrasena,
            string hashAlmacenado)
        {
            // Comprueba que los valores no estén vacíos.
            if (string.IsNullOrEmpty(contrasena) ||
                string.IsNullOrEmpty(hashAlmacenado))
            {
                return false;
            }

            try
            {
                // Separa la sal y el hash almacenados.
                string[] partes = hashAlmacenado.Split(':');

                if (partes.Length != 2)
                {
                    return false;
                }

                byte[] sal = Convert.FromBase64String(partes[0]);
                byte[] hashOriginal = Convert.FromBase64String(partes[1]);

                // Comprueba que tengan los tamaños esperados.
                if (sal.Length != 16 || hashOriginal.Length != 32)
                {
                    return false;
                }

                // Genera nuevamente el hash usando la misma sal.
                byte[] hashNuevo;

                using (Rfc2898DeriveBytes pbkdf2 =
                       new Rfc2898DeriveBytes(
                           contrasena,
                           sal,
                           600000,
                           HashAlgorithmName.SHA256))
                {
                    hashNuevo = pbkdf2.GetBytes(32);
                }

                // Compara los hashes sin detenerse
                // al encontrar el primer byte diferente.
                int diferencia = 0;

                for (int i = 0; i < hashOriginal.Length; i++)
                {
                    diferencia |= hashOriginal[i] ^ hashNuevo[i];
                }

                return diferencia == 0;
            }
            catch (FormatException)
            {
                // El hash almacenado tiene un formato inválido.
                return false;
            }
            catch (ArgumentException)
            {
                // Los parámetros del hash son inválidos.
                return false;
            }
        }

        /// <summary>
        /// Crea un usuario después de validar sus datos.
        /// Es privado para evitar registros no autorizados.
        /// </summary>
        private bool CrearUsuarioInterno(
            string nombres,
            string apellidos,
            string nombreUsuario,
            string contrasena,
            RolUsuario rol)
        {
            // Valida los campos obligatorios.
            if (!Validador.CampoObligatorio(nombres) ||
                !Validador.CampoObligatorio(apellidos) ||
                !Validador.CampoObligatorio(nombreUsuario))
            {
                return false;
            }

            // Valida la longitud mínima de la contraseña.
            if (string.IsNullOrEmpty(contrasena) ||
                contrasena.Length < 8)
            {
                return false;
            }

            // Verifica que el rol sea válido.
            if (!Enum.IsDefined(typeof(RolUsuario), rol))
            {
                return false;
            }

            // Evita nombres de usuario duplicados.
            if (ConsultarUsuario(nombreUsuario) != null)
            {
                return false;
            }

            var datos = AlmacenDatos.Instancia;

            // Genera un ID único durante la ejecución.
            int nuevoId = datos.Usuarios.Count == 0
                ? 1
                : datos.Usuarios.Max(u => u.Id) + 1;

            // Protege la contraseña mediante un hash.
            string hash = GenerarHashContrasena(contrasena);

            // Crea la cuenta.
            Usuario usuario = new Usuario(
                nuevoId,
                nombres.Trim(),
                apellidos.Trim(),
                nombreUsuario.Trim(),
                hash,
                rol);

            // Guarda el usuario en la lista.
            datos.Usuarios.Add(usuario);

            // Registra la operación.
            datos.RegistrarOperacion(
                "Usuario registrado: " + usuario.NombreUsuario +
                " - Rol: " + usuario.Rol);

            return true;
        }

        /// <summary>
        /// Permite registrar usuarios únicamente
        /// a un administrador autenticado.
        /// </summary>
        public bool RegistrarUsuario(
            string nombres,
            string apellidos,
            string nombreUsuario,
            string contrasena,
            RolUsuario rol)
        {
            // Verifica que el usuario conectado
            // tenga permisos de administrador.
            if (!SesionActual.EsAdministrador() ||
                !SesionActual.Usuario.Activo)
            {
                return false;
            }

            // Reutiliza la lógica interna de registro.
            return CrearUsuarioInterno(
                nombres,
                apellidos,
                nombreUsuario,
                contrasena,
                rol);
        }

        /// <summary>
        /// Autentica a un usuario mediante sus credenciales.
        /// Devuelve true si el inicio de sesión fue exitoso.
        /// </summary>
        public bool IniciarSesion(string nombreUsuario, string contrasena)
        {
            // Verifica que ambos campos estén completos.
            if (!Validador.CampoObligatorio(nombreUsuario) ||
                string.IsNullOrEmpty(contrasena))
            {
                return false;
            }

            // Busca al usuario registrado.
            Usuario usuario = ConsultarUsuario(nombreUsuario);

            // Comprueba que exista y que su cuenta esté activa.
            if (usuario == null || !usuario.Activo)
            {
                return false;
            }

            // Verifica la contraseña contra el hash almacenado.
            if (!VerificarContrasena(contrasena, usuario.HashContrasena))
            {
                return false;
            }

            // Guarda al usuario que inició sesión correctamente.
            SesionActual.IniciarSesion(usuario);

            // Registra el inicio de sesión en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Inicio de sesión: " + usuario.NombreUsuario);

            return true;
        }

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// y registra la operación en el historial.
        /// </summary>
        public bool CerrarSesion()
        {
            // Verifica si existe una sesión activa.
            if (!SesionActual.HaySesionActiva())
            {
                return false;
            }

            // Registra quién está cerrando sesión.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Cierre de sesión: " +
                SesionActual.Usuario.NombreUsuario);

            // Elimina al usuario de la sesión actual.
            SesionActual.CerrarSesion();

            return true;
        }

        /// <summary>
        /// Carga las cuentas iniciales de demostración.
        /// Solo se ejecuta cuando no existen usuarios.
        /// </summary>
        public void CargarUsuariosIniciales()
        {
            // Evita duplicar las cuentas iniciales.
            if (AlmacenDatos.Instancia.Usuarios.Count > 0)
            {
                return;
            }

            // Cuenta administrador de ejemplo.
            CrearUsuarioInterno(
                "Ismael",
                "Espinoza",
                "admin1",
                "Isma12345!",
                RolUsuario.Administrador);

            // Cuenta de empleado de ejemplo.
            CrearUsuarioInterno(
                "Laura",
                "Mora",
                "empleado1",
                "Lau12345!",
                RolUsuario.Empleado);
        }

    }
}
