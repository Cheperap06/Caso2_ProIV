using System.Linq;
using System.Text.RegularExpressions;
using Caso2_ProIV.Models;
using Caso2_ProIV.Services;

namespace Caso2_ProIV.Helpers
{
    /// <summary>
    /// Reglas de validación del sistema. No usa controles visuales:
    /// solo recibe datos y responde true/false, así se reutiliza en cualquier formulario.
    /// </summary>
    public static class Validador
    {
        private const int LongitudTelefono = 8;

        private static readonly Regex PatronCorreo =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",  
                     RegexOptions.Compiled);

        // ---------- Formato ----------

        /// <summary>Campo obligatorio: no vacío ni solo espacios.</summary>
        public static bool CampoObligatorio(string valor)
        {
            return !string.IsNullOrWhiteSpace(valor);
        }

        /// <summary>Formato de correo: algo@dominio.ext</summary>
        public static bool CorreoValido(string correo)
        {
            return CampoObligatorio(correo) && PatronCorreo.IsMatch(correo.Trim());
        }

        /// <summary>
        /// Teléfono de 8 dígitos. Acepta "88881111" o "8888-1111"
        /// (ignora guiones, espacios y los guiones bajos de una MaskedTextBox vacía).
        /// </summary>
        public static bool TelefonoValido(string telefono)
        {
            if (!CampoObligatorio(telefono))
            {
                return false;
            }

            string soloDigitos = new string(telefono.Where(char.IsDigit).ToArray());
            bool tieneCaracteresInvalidos = telefono.Any(c => !char.IsDigit(c) && c != '-' && c != ' ');

            return !tieneCaracteresInvalidos && soloDigitos.Length == LongitudTelefono;
        }

        // ---------- Números ----------

        public static bool PrecioValido(decimal precio)
        {
            return precio > 0;
        }

        public static bool CantidadPositiva(int cantidad)
        {
            return cantidad > 0;
        }

        /// <summary>El inventario no puede quedar negativo al actualizar existencias.</summary>
        public static bool ExistenciaValida(int cantidad)
        {
            return cantidad >= 0;
        }

        // ---------- Duplicados e inventario ----------

        /// <summary>
        /// True si ya existe un cliente con esa identificación.
        /// Al MODIFICAR, pasa la identificación actual para que no se compare consigo mismo.
        /// </summary>
        public static bool IdentificacionDuplicada(string identificacion, string identificacionActual = null)
        {
            string buscada = (identificacion ?? "").Trim();
            string actual = (identificacionActual ?? "").Trim();

            return AlmacenDatos.Instancia.Clientes.Any(c =>
                c.Identificacion == buscada && c.Identificacion != actual);
        }

        /// <summary>True si ya existe un producto con ese código (el Dictionary ignora mayúsculas).</summary>
        public static bool CodigoDuplicado(string codigo)
        {
            return AlmacenDatos.Instancia.Productos.ContainsKey((codigo ?? "").Trim());
        }

        /// <summary>True si ya existe esa categoría (el HashSet ignora mayúsculas).</summary>
        public static bool CategoriaDuplicada(string categoria)
        {
            return AlmacenDatos.Instancia.Categorias.Contains((categoria ?? "").Trim());
        }

        /// <summary>True si el producto existe y tiene unidades suficientes para la cantidad pedida.</summary>
        public static bool HayInventarioSuficiente(
    Producto producto, int cantidadSolicitada)
        {
            return producto != null &&
                   cantidadSolicitada > 0 &&
                   cantidadSolicitada <= producto.CantidadInventario;
        }
    }
}
