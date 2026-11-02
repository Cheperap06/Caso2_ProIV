
using System;
using System.Collections.Generic;
using System.Linq;
using Caso2_ProIV.Models;
using Caso2_ProIV.Helpers;

namespace Caso2_ProIV.Services
{
    /// <summary>
    /// Administra las operaciones relacionadas con las ventas.
    /// Verifica los productos y el inventario antes de
    /// registrar una factura.
    /// </summary>
    public class VentaService
    {


        /// <summary>
        /// Consulta una venta mediante su número de factura.
        /// Devuelve la venta encontrada o null si no existe.
        /// Permite registrar la consulta en el historial.
        /// </summary>
        public Venta ConsultarVenta(
            int numeroFactura,
            bool registrarEnHistorial = false)
        {
            // Verifica que el número de factura sea positivo.
            if (numeroFactura <= 0)
            {
                return null;
            }

            // Busca la factura utilizando LINQ.
            Venta venta = AlmacenDatos.Instancia.Ventas
                .FirstOrDefault(v =>
                    v.NumeroFactura == numeroFactura);

            // Registra la consulta cuando es solicitada.
            if (registrarEnHistorial)
            {
                AlmacenDatos.Instancia.RegistrarOperacion(
                    "Consulta ejecutada - Factura #" + numeroFactura);
            }

            // Devuelve la venta encontrada o null.
            return venta;
        }

        /// <summary>
        /// Registra una venta, valida el cliente y los productos,
        /// descuenta el inventario y guarda la factura.
        /// Devuelve la venta registrada o null si no fue válida.
        /// </summary>
        public Venta RegistrarVenta(
            string identificacionCliente,
            List<DetalleVenta> detalles)
        {
            // Verifica que la identificación sea obligatoria.
            if (!Validador.CampoObligatorio(identificacionCliente))
            {
                return null;
            }

            // Busca al cliente registrado.
            ClienteService clienteService = new ClienteService();
            Cliente cliente = clienteService.ConsultarCliente(
                identificacionCliente);

            if (cliente == null || cliente.Estado != EstadoCliente.Activo)
            {
                return null;
            }

            // Comprueba que la factura contenga productos.
            if (detalles == null || detalles.Count == 0)
            {
                return null;
            }

            // Obtiene el almacén central.
            AlmacenDatos datos = AlmacenDatos.Instancia;

            // Acumula las cantidades solicitadas por código.
            // Así evitamos superar el inventario cuando un
            // producto aparece varias veces en la misma factura.
            Dictionary<string, int> cantidades =
                new Dictionary<string, int>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (DetalleVenta detalle in detalles)
            {
                // Valida el detalle y su cantidad.
                if (detalle == null ||
                    !Validador.CampoObligatorio(detalle.CodigoProducto) ||
                    !Validador.CantidadPositiva(detalle.Cantidad))
                {
                    return null;
                }

                string codigo = detalle.CodigoProducto.Trim();

                // Comprueba que el producto exista.
                if (!datos.Productos.ContainsKey(codigo))
                {
                    return null;
                }

                // Acumula la cantidad solicitada.
                if (!cantidades.ContainsKey(codigo))
                {
                    cantidades[codigo] = 0;
                }

                // Evita que la suma exceda el límite de int.
                if (detalle.Cantidad >
                    int.MaxValue - cantidades[codigo])
                {
                    return null;
                }

                cantidades[codigo] += detalle.Cantidad;
            }

            // Valida el inventario total solicitado
            // antes de descontar cualquier producto.
            foreach (var item in cantidades)
            {
                Producto producto = datos.Productos[item.Key];

                if (!Validador.HayInventarioSuficiente(
                    producto, item.Value))
                {
                    return null;
                }
            }

            // Obtiene el siguiente número de factura.
            int numeroFactura = datos.ObtenerSiguienteNumeroFactura();

            // Crea la factura.
            Venta venta = new Venta(numeroFactura, cliente);

            // Agrega cada producto utilizando su precio actual.
            foreach (var item in cantidades)
            {
                Producto producto = datos.Productos[item.Key];

                DetalleVenta detalle = new DetalleVenta(
                    producto, item.Value);

                venta.AgregarDetalle(detalle);
            }

            // Descuenta las unidades vendidas del inventario.
            foreach (var item in cantidades)
            {
                datos.Productos[item.Key].CantidadInventario
                    -= item.Value;
            }

            // Guarda la factura en la lista de ventas.
            datos.Ventas.Add(venta);

            // Avanza el consecutivo de las facturas.
            datos.ConfirmarNumeroFactura();

            // Registra la operación en el historial.
            datos.RegistrarOperacion(
                "Venta registrada - Factura #" +
                venta.NumeroFactura +
                " - Total: " + venta.Total);

            return venta;
        }

        /// <summary>
        /// Devuelve una lista con todas las ventas registradas
        /// para consultar el historial de facturas.
        /// </summary>

        public List<Venta> ObtenerVentas(
            bool registrarEnHistorial = false)
        {
            // Obtiene las ventas registradas.
            List<Venta> ventas =
                AlmacenDatos.Instancia.Ventas.ToList();

            // Registra la consulta cuando es solicitada.
            if (registrarEnHistorial)
            {
                AlmacenDatos.Instancia.RegistrarOperacion(
                    "Consulta ejecutada - Listado de ventas");
            }

            return ventas;
        }


    }
}
