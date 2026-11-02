
using System.Collections.Generic;
using System.Linq;

namespace Caso2_ProIV.Services
{
    /// <summary>
    /// Permite consultar el historial de operaciones
    /// realizadas durante la ejecución del sistema.
    /// </summary>
    public class HistorialService
    {
        /// <summary>
        /// Obtiene el historial desde la operación más reciente.
        /// Solo permite consultar a administradores activos.
        /// </summary>
        public List<string> ObtenerHistorial()
        {
            // Comprueba que exista un administrador activo.
            if (!SesionActual.EsAdministrador() ||
                !SesionActual.Usuario.Activo)
            {
                return new List<string>();
            }

            // Devuelve los registros en orden LIFO,
            // sin eliminarlos de la pila.
            return AlmacenDatos.Instancia.Historial.ToList();
        }


        /// <summary>
        /// Devuelve la cantidad de operaciones registradas.
        /// Solo permite consultar a administradores activos.
        /// </summary>
        public int ObtenerCantidadOperaciones()
        {
            // Comprueba que exista un administrador activo.
            if (!SesionActual.EsAdministrador() ||
                !SesionActual.Usuario.Activo)
            {
                return 0;
            }

            // Devuelve la cantidad de registros de la pila.
            return AlmacenDatos.Instancia.Historial.Count;
        }


    }
}
