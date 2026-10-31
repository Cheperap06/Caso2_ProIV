namespace Caso2_ProIV.Models
{
    /// <summary>
    /// Una línea de la factura: un producto vendido con su cantidad.
    /// Guarda el precio y el nombre del momento de la venta, así el historial
    /// no cambia si luego se modifica el producto.
    /// </summary>
    public class DetalleVenta
    {
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }

        public DetalleVenta()
        {
        }

        public DetalleVenta(Producto producto, int cantidad)
        {
            CodigoProducto = producto.Codigo;
            NombreProducto = producto.Nombre;
            PrecioUnitario = producto.PrecioUnitario;
            Cantidad = cantidad;
        }
    }
}
