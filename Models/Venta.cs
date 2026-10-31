using System;
using System.Collections.Generic;
using System.Linq;

namespace Caso2_ProIV.Models
{
    /// <summary>
    /// Representa una factura. Se almacena en un List&lt;Venta&gt;.
    /// Subtotal, IVA y Total se calculan a partir de los detalles.
    /// </summary>
    public class Venta
    {
        public const decimal TasaIva = 0.13m;

        public int NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public Cliente Cliente { get; set; }
        public List<DetalleVenta> Detalles { get; private set; }

        public decimal Subtotal
        {
            get { return Detalles.Sum(d => d.Subtotal); }
        }

        public decimal Iva
        {
            get { return Math.Round(Subtotal * TasaIva, 2); }
        }

        public decimal Total
        {
            get { return Subtotal + Iva; }
        }

        public Venta()
        {
            Fecha = DateTime.Now;
            Detalles = new List<DetalleVenta>();
        }

        public Venta(int numeroFactura, Cliente cliente) : this()
        {
            NumeroFactura = numeroFactura;
            Cliente = cliente;
        }

        public void AgregarDetalle(DetalleVenta detalle)
        {
            Detalles.Add(detalle);
        }
    }
}
