namespace Caso2_ProIV.Models
{
    /// <summary>
    /// Representa un producto del inventario.
    /// Se almacena en un Dictionary&lt;string, Producto&gt; cuya clave es el Codigo.
    /// </summary>
    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string Marca { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int CantidadInventario { get; set; }

        /// <summary>
        /// El estado se calcula solo: Agotado cuando no quedan unidades.
        /// </summary>
        public EstadoProducto Estado
        {
            get { return CantidadInventario > 0 ? EstadoProducto.Disponible : EstadoProducto.Agotado; }
        }

        public Producto()
        {
        }

        public Producto(string codigo, string nombre, string categoria, string marca,
                        decimal precioUnitario, int cantidadInventario)
        {
            Codigo = codigo;
            Nombre = nombre;
            Categoria = categoria;
            Marca = marca;
            PrecioUnitario = precioUnitario;
            CantidadInventario = cantidadInventario;
        }

        public override string ToString()
        {
            return Codigo + " - " + Nombre;
        }
    }
}
