
using System;
using System.Collections.Generic;
using System.Linq;
using Caso2_ProIV.Models;
using Caso2_ProIV.Helpers;

namespace Caso2_ProIV.Services
{
    /// <summary>
    /// Administra las operaciones relacionadas con
    /// los productos y el inventario.
    /// </summary>
    public class ProductoService
    {
        /// <summary>
        /// Obtiene el prefijo correspondiente a una categoría.
        /// Se utiliza para generar códigos de productos organizados
        /// según la categoría a la que pertenecen.
        /// </summary>
        private string ObtenerPrefijoCategoria(string categoria)
        {
            // Elimina espacios y convierte el texto a mayúsculas
            // para facilitar la comparación de las categorías.
            switch (categoria.Trim().ToUpperInvariant())
            {
                case "LÍNEA BLANCA":
                case "LINEA BLANCA":
                    return "LB";

                case "ELECTRODOMÉSTICOS":
                case "ELECTRODOMESTICOS":
                    return "ED";

                case "ELECTRÓNICA":
                case "ELECTRONICA":
                    return "EL";

                case "TECNOLOGÍA":
                case "TECNOLOGIA":
                    return "TEC";

                case "AUDIO":
                    return "AUD";

                case "VIDEO":
                    return "VID";

                // Si no corresponde a ninguna categoría definida,
                // se asigna el prefijo general.
                default:
                    return "GEN";
            }
        }

        /// <summary>
        /// Genera un código único para un producto según su categoría.
        /// Guarda el último consecutivo utilizado para evitar
        /// reutilizar códigos, incluso después de eliminar productos.
        /// </summary>
        private string GenerarCodigoProducto(string categoria)
        {
            // Obtiene el prefijo correspondiente a la categoría.
            string prefijo = ObtenerPrefijoCategoria(categoria);

            // Accede al almacén central del sistema.
            AlmacenDatos datos = AlmacenDatos.Instancia;

            // Obtiene el último consecutivo de este prefijo.
            // Si todavía no existe, comienza desde cero.
            int ultimoConsecutivo = 0;

            datos.ConsecutivosProductos.TryGetValue(
                prefijo, out ultimoConsecutivo);

            // Busca el siguiente consecutivo disponible.
            string codigo;

            do
            {
                ultimoConsecutivo++;

                // Forma el código con tres dígitos como mínimo.
                codigo = prefijo + "-" +
                         ultimoConsecutivo.ToString("D3");

            } while (datos.Productos.ContainsKey(codigo));

            // Guarda el último consecutivo utilizado.
            datos.ConsecutivosProductos[prefijo] = ultimoConsecutivo;

            // Devuelve el código generado.
            return codigo;
        }


        /// <summary>
        /// Registra un producto nuevo, valida sus datos
        /// y genera automáticamente su código según la categoría.
        /// </summary>
        public bool RegistrarProducto(Producto producto)
        {
            // Verifica que el producto exista.
            if (producto == null)
            {
                return false;
            }

            // Valida que los datos obligatorios sean correctos.
            if (!Validador.CampoObligatorio(producto.Nombre) ||
                !Validador.CampoObligatorio(producto.Categoria) ||
                !Validador.CampoObligatorio(producto.Marca) ||
                !Validador.PrecioValido(producto.PrecioUnitario) ||
                !Validador.ExistenciaValida(producto.CantidadInventario))
            {
                return false;
            }

            // Elimina espacios innecesarios.
            producto.Nombre = producto.Nombre.Trim();
            producto.Categoria = producto.Categoria.Trim();
            producto.Marca = producto.Marca.Trim();

            // Genera el código único según su categoría.
            producto.Codigo = GenerarCodigoProducto(producto.Categoria);

            // Busca si la categoría ya existe en el almacén.
            // La comparación no distingue mayúsculas de minúsculas.
            string categoriaExistente = AlmacenDatos.Instancia.Categorias
                .FirstOrDefault(c => string.Equals(
                    c, producto.Categoria,
                    StringComparison.OrdinalIgnoreCase));

            // Si existe, utiliza el nombre que ya estaba registrado.
            if (categoriaExistente != null)
            {
                producto.Categoria = categoriaExistente;
            }
            else
            {
                // Si no existe, agrega la nueva categoría.
                AlmacenDatos.Instancia.Categorias.Add(producto.Categoria);
            }

            // Guarda el producto en el Dictionary.
            AlmacenDatos.Instancia.Productos.Add(
                producto.Codigo, producto);

            // Registra la operación en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Producto registrado: " + producto.Nombre +
                " (" + producto.Codigo + ")");

            return true;
        }

        /// <summary>
        /// Consulta un producto mediante su código.
        /// Devuelve el producto encontrado o null si no existe.
        /// </summary>
        public Producto ConsultarProducto(string codigo)
        {
            // Verifica que el código no esté vacío.
            if (!Validador.CampoObligatorio(codigo))
            {
                return null;
            }

            // Elimina espacios innecesarios.
            codigo = codigo.Trim();

            // Busca el producto utilizando su código como clave.
            AlmacenDatos.Instancia.Productos.TryGetValue(
                codigo, out Producto producto);

            // Devuelve el producto encontrado o null.
            return producto;
        }

        /// <summary>
        /// Devuelve una lista con todos los productos
        /// registrados en el inventario.
        /// </summary>
        public List<Producto> ObtenerProductos()
        {
            // Obtiene los productos almacenados en el Dictionary
            // y los convierte en una lista.
            return AlmacenDatos.Instancia.Productos.Values.ToList();
        }

        /// <summary>
        /// Modifica la información de un producto existente.
        /// Conserva su código original y devuelve true
        /// si la modificación se realiza correctamente.
        /// </summary>
        public bool ModificarProducto(Producto producto)
        {
            // Verifica que el producto exista.
            if (producto == null ||
                !Validador.CampoObligatorio(producto.Codigo))
            {
                return false;
            }

            // Busca el producto registrado por su código.
            Producto productoExistente = ConsultarProducto(producto.Codigo);

            if (productoExistente == null)
            {
                return false;
            }

            // Valida la nueva información del producto.
            if (!Validador.CampoObligatorio(producto.Nombre) ||
                !Validador.CampoObligatorio(producto.Categoria) ||
                !Validador.CampoObligatorio(producto.Marca) ||
                !Validador.PrecioValido(producto.PrecioUnitario))
            {
                return false;
            }

            // Elimina espacios innecesarios.
            string nombre = producto.Nombre.Trim();
            string categoria = producto.Categoria.Trim();
            string marca = producto.Marca.Trim();

            // Busca la categoría para conservar su nombre registrado.
            string categoriaExistente = AlmacenDatos.Instancia.Categorias
                .FirstOrDefault(c => string.Equals(
                    c, categoria, StringComparison.OrdinalIgnoreCase));

            if (categoriaExistente != null)
            {
                categoria = categoriaExistente;
            }
            else
            {
                AlmacenDatos.Instancia.Categorias.Add(categoria);
            }

            // Actualiza la información general del producto.
            // No modifica el código ni las existencias.
            productoExistente.Nombre = nombre;
            productoExistente.Categoria = categoria;
            productoExistente.Marca = marca;
            productoExistente.PrecioUnitario = producto.PrecioUnitario;
          

            // Registra la operación en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Producto modificado: " + productoExistente.Nombre +
                " (" + productoExistente.Codigo + ")");

            return true;
        }

        /// <summary>
        /// Actualiza la cantidad de unidades disponibles
        /// de un producto existente.
        /// Devuelve true si la actualización fue exitosa.
        /// </summary>
        public bool ActualizarExistencias(string codigo, int nuevaCantidad)
        {
            // Verifica que el código no esté vacío.
            if (!Validador.CampoObligatorio(codigo))
            {
                return false;
            }

            // Busca el producto por su código.
            Producto producto = ConsultarProducto(codigo);

            if (producto == null)
            {
                return false;
            }

            // Verifica que la nueva cantidad no sea negativa.
            if (!Validador.ExistenciaValida(nuevaCantidad))
            {
                return false;
            }

            // Actualiza las unidades disponibles.
            producto.CantidadInventario = nuevaCantidad;

            // Registra la operación en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Inventario actualizado: " + producto.Nombre +
                " (" + producto.Codigo + ") - Existencias: " +
                nuevaCantidad);

            return true;
        }

        /// <summary>
        /// Adiciona unidades al inventario de un producto existente.
        /// Suma la cantidad recibida a las existencias actuales.
        /// Devuelve true si la operación se realiza correctamente.
        /// </summary>
        public bool AdicionarInventario(string codigo, int cantidadAdicionar)
        {
            // Verifica que el código no esté vacío.
            if (!Validador.CampoObligatorio(codigo))
            {
                return false;
            }

            // Busca el producto por su código.
            Producto producto = ConsultarProducto(codigo);

            if (producto == null)
            {
                return false;
            }

            // Verifica que la cantidad a adicionar sea positiva.
            if (!Validador.CantidadPositiva(cantidadAdicionar))
            {
                return false;
            }

            // Comprueba que la suma no supere el límite de int.
            if (cantidadAdicionar > int.MaxValue - producto.CantidadInventario)
            {
                return false;
            }

            // Guarda la cantidad anterior para el historial.
            int cantidadAnterior = producto.CantidadInventario;

            // Suma las unidades nuevas al inventario existente.
            producto.CantidadInventario += cantidadAdicionar;

            // Registra la operación en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Existencias adicionadas: " + producto.Nombre +
                " (" + producto.Codigo + ")" +
                " - Anterior: " + cantidadAnterior +
                " - Adicionadas: " + cantidadAdicionar +
                " - Total: " + producto.CantidadInventario);

            return true;
        }

        /// <summary>
        /// Elimina un producto del inventario.
        /// No permite eliminar productos que tengan ventas registradas.
        /// Devuelve true si la eliminación fue exitosa.
        /// </summary>
        public bool EliminarProducto(string codigo)
        {
            // Verifica que el código no esté vacío.
            if (!Validador.CampoObligatorio(codigo))
            {
                return false;
            }

            // Busca el producto mediante su código.
            Producto producto = ConsultarProducto(codigo);

            if (producto == null)
            {
                return false;
            }

            // Comprueba si el producto aparece en alguna factura.
            bool tieneVentas = AlmacenDatos.Instancia.Ventas
                .Any(v => v.Detalles.Any(d =>
                    string.Equals(d.CodigoProducto, producto.Codigo,
                                  StringComparison.OrdinalIgnoreCase)));

            // No permite eliminar productos con ventas registradas.
            if (tieneVentas)
            {
                return false;
            }

            // Elimina el producto del Dictionary.
            bool eliminado = AlmacenDatos.Instancia.Productos
                .Remove(producto.Codigo);

            if (!eliminado)
            {
                return false;
            }

            // Registra la eliminación en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Producto eliminado: " + producto.Nombre +
                " (" + producto.Codigo + ")");

            return true;
        }

    }
}
