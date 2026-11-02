using System;

namespace Caso2_ProIV.Models
{
    /// <summary>
    /// Representa a un cliente de ElectroHogar S.A.
    /// Se almacena en un List&lt;Cliente&gt;.
    /// </summary>
    public class Cliente
    {
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public EstadoCliente Estado { get; set; }

        public Cliente()
        {
            FechaRegistro = DateTime.Now;
            Estado = EstadoCliente.Activo;
        }

        public Cliente(string identificacion, string nombreCompleto, string telefono,
                       string correo, string direccion, DateTime fechaRegistro,
                       EstadoCliente estado)
        {
            Identificacion = identificacion;
            NombreCompleto = nombreCompleto;
            Telefono = telefono;
            Correo = correo;
            Direccion = direccion;
            FechaRegistro = fechaRegistro;
            Estado = estado;
        }

        /// <summary>Texto que mostrarán los ComboBox y ListBox.</summary>
        public override string ToString()
        {
            return Identificacion + " - " + NombreCompleto;
        }
    }
}
