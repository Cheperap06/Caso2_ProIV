using System;
using System.Collections.Generic;
using Caso2_ProIV.Models;
using Caso2_ProIV.Services;

namespace Caso2_ProIV.Services
{
    /// <summary>
    /// Almacén central de datos en memoria (no hay base de datos).
    /// Es un Singleton: todos los formularios usan la MISMA instancia
    /// (AlmacenDatos.Instancia), por eso la información no se pierde
    /// al navegar entre formularios.
    /// </summary>
    public class AlmacenDatos
    {
        // ---- Instancia única ----
        public static AlmacenDatos Instancia { get; } = new AlmacenDatos();

        // ---- Colecciones (una por tipo de información) ----

        /// <summary>Clientes registrados.</summary>
        public List<Cliente> Clientes { get; private set; }

        /// <summary>Inventario. La clave es el código único del producto.</summary>
        public Dictionary<string, Producto> Productos { get; private set; }

        /// <summary>Categorías sin repetir.</summary>
        public HashSet<string> Categorias { get; private set; }

        /// <summary>Ventas (facturas) realizadas.</summary>
        public List<Venta> Ventas { get; private set; }

        /// <summary>Clientes en espera, atendidos por orden de llegada (FIFO).</summary>
        public Queue<Cliente> ColaAtencion { get; private set; }

        /// <summary>Historial de operaciones. Recorrerlo muestra primero la más reciente (LIFO).</summary>
        public Stack<string> Historial { get; private set; }

        /// <summary>
        /// Almacena el último consecutivo utilizado para cada prefijo.
        /// Permite evitar la reutilización de códigos de productos eliminados.
        /// </summary>
        public Dictionary<string, int> ConsecutivosProductos { get; private set; }
        /// <summary>
        /// Almacena los usuarios registrados en el sistema
        /// Permite ingresar y hacer cambios según su rol
        /// </summary>
        public List<Usuario> Usuarios { get; private set; }

        private int _siguienteNumeroFactura;

        private AlmacenDatos()
        {
            Clientes = new List<Cliente>();
            Usuarios = new List<Usuario>();

            // OrdinalIgnoreCase: "audio" y "Audio" cuentan como la misma clave/categoría.
            Productos = new Dictionary<string, Producto>(StringComparer.OrdinalIgnoreCase);
            Categorias = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Ventas = new List<Venta>();
            ColaAtencion = new Queue<Cliente>();
            Historial = new Stack<string>();

            _siguienteNumeroFactura = 1;

            // Inicializa los consecutivos de los códigos de productos.
            ConsecutivosProductos = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);

            CargarCategoriasIniciales();
        }

        private void CargarCategoriasIniciales()
        {
            Categorias.Add("Línea Blanca");
            Categorias.Add("Electrodomésticos");
            Categorias.Add("Electrónica");
            Categorias.Add("Tecnología");
            Categorias.Add("Audio");
            Categorias.Add("Video");
        }

        /// <summary>
        /// Registra una operación indicando la fecha,
        /// hora, usuario responsable y descripción.
        /// </summary>
        public void RegistrarOperacion(string descripcion)
        {
            // Identifica quién realizó la operación.
            string responsable = "Sistema";

            // Si existe una sesión activa, obtiene los datos.
            if (SesionActual.HaySesionActiva())
            {
                Usuario usuario = SesionActual.Usuario;

                responsable = usuario.NombreUsuario +
                              " (" + usuario.Rol + ")";
            }

            // Guarda la operación en la pila del historial.
            Historial.Push(
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") +
                " - " + responsable +
                " - " + descripcion);
        }


        /// <summary>Devuelve el número de factura que se asignará a la próxima venta.</summary>
        public int ObtenerSiguienteNumeroFactura()
        {
            return _siguienteNumeroFactura;
        }

        /// <summary>Se llama una vez que la venta quedó guardada, para avanzar el consecutivo.</summary>
        public void ConfirmarNumeroFactura()
        {
            _siguienteNumeroFactura++;
        }

        /// <summary>
        /// Opcional: carga datos de prueba para probar reportes y ventas sin escribir todo a mano.
        /// No se llama automáticamente.
        /// </summary>
        public void CargarDatosDeEjemplo()
        {
            if (Clientes.Count > 0 || Productos.Count > 0)
            {
                return;
            }

            Clientes.Add(new Cliente("101110111", "Ana Mora Rojas", "88881111", "ana.mora@correo.com",
                                     "San José, Escazú", DateTime.Now, EstadoCliente.Activo));
            Clientes.Add(new Cliente("202220222", "Luis Vargas Solano", "88882222", "luis.vargas@correo.com",
                                     "Heredia Centro", DateTime.Now, EstadoCliente.Activo));
            Clientes.Add(new Cliente("303330333", "María Jiménez Soto", "88883333", "maria.jimenez@correo.com",
                                     "Alajuela, Grecia", DateTime.Now, EstadoCliente.Inactivo));

            AgregarProductoEjemplo(new Producto(
    "LB-001", "Refrigeradora 14 pies",
    "Línea Blanca", "Samsung", 450000m, 8));

            AgregarProductoEjemplo(new Producto(
                "VID-001", "Televisor 50 pulgadas",
                "Video", "LG", 380000m, 3));

            AgregarProductoEjemplo(new Producto(
                "AUD-001", "Parlante Bluetooth",
                "Audio", "JBL", 45000m, 20));

            AgregarProductoEjemplo(new Producto(
                "TEC-001", "Laptop 15 pulgadas",
                "Tecnología", "HP", 520000m, 0));
        }

        /// <summary>
        /// Agrega un producto de ejemplo al inventario.
        /// También actualiza el consecutivo de su prefijo
        /// para evitar generar códigos ya utilizados.
        /// </summary>
        private void AgregarProductoEjemplo(Producto producto)
        {
            // Agrega la categoría si todavía no existe.
            Categorias.Add(producto.Categoria);

            // Agrega el producto al inventario.
            Productos.Add(producto.Codigo, producto);

            // Separa el prefijo del número consecutivo.
            // Ejemplo: AUD-001 se divide en AUD y 001.
            string[] partes = producto.Codigo.Split('-');

            // Verifica que el código tenga dos partes
            // y que la segunda sea un número válido.
            if (partes.Length == 2 &&
                int.TryParse(partes[1], out int numero))
            {
                string prefijo = partes[0];

                // Obtiene el último consecutivo registrado.
                int ultimo = 0;
                ConsecutivosProductos.TryGetValue(prefijo, out ultimo);

                // Conserva siempre el consecutivo mayor.
                if (numero > ultimo)
                {
                    ConsecutivosProductos[prefijo] = numero;
                }
            }
        }

    }
}
