
using System;
using System.Linq;
using Caso2_ProIV.Models;
using Caso2_ProIV.Helpers;

namespace Caso2_ProIV.Services
{
    /// <summary>
    /// Administra la cola de atención de los clientes.
    /// Utiliza Queue para respetar el orden de llegada (FIFO).
    /// </summary>
    public class ColaAtencionService
    {

        /// <summary>
        /// Agrega un cliente registrado a la cola de atención.
        /// Impide agregar clientes inexistentes, inactivos
        /// o que ya estén esperando.
        /// </summary>
        public bool AgregarClienteCola(string identificacion)
        {
            // Verifica que la identificación no esté vacía.
            if (!Validador.CampoObligatorio(identificacion))
            {
                return false;
            }

            // Consulta al cliente registrado en el sistema.
            ClienteService clienteService = new ClienteService();
            Cliente cliente = clienteService.ConsultarCliente(identificacion);

            // Verifica que exista y esté activo.
            if (cliente == null || cliente.Estado != EstadoCliente.Activo)
            {
                return false;
            }

            // Comprueba que el cliente no esté ya en la cola.
            bool estaEnCola = AlmacenDatos.Instancia.ColaAtencion
                .Any(c => c.Identificacion == cliente.Identificacion);

            if (estaEnCola)
            {
                return false;
            }

            // Agrega el cliente al final de la cola.
            AlmacenDatos.Instancia.ColaAtencion.Enqueue(cliente);

            // Registra la operación en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Cliente agregado a la cola: " + cliente.NombreCompleto);

            return true;
        }


        /// <summary>
        /// Atiende al primer cliente de la cola.
        /// Devuelve el cliente atendido o null si la cola está vacía.
        /// </summary>
        public Cliente AtenderSiguiente()
        {
            // Comprueba si hay clientes esperando.
            if (AlmacenDatos.Instancia.ColaAtencion.Count == 0)
            {
                return null;
            }

            // Retira al primer cliente de la cola.
            Cliente cliente = AlmacenDatos.Instancia.ColaAtencion.Dequeue();

            // Registra la atención en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Cliente atendido: " + cliente.NombreCompleto);

            // Devuelve el cliente que fue atendido.
            return cliente;
        }

        /// <summary>
        /// Consulta quién es el siguiente cliente en ser atendido.
        /// Devuelve el cliente o null si la cola está vacía.
        /// No elimina al cliente de la cola.
        /// </summary>
        public Cliente ConsultarSiguiente()
        {
            // Verifica si hay clientes esperando.
            if (AlmacenDatos.Instancia.ColaAtencion.Count == 0)
            {
                return null;
            }

            // Obtiene al primer cliente sin retirarlo de la cola.
            return AlmacenDatos.Instancia.ColaAtencion.Peek();
        }

        /// <summary>
        /// Obtiene la cantidad total de clientes
        /// que se encuentran esperando en la cola.
        /// </summary>
        public int ObtenerCantidadEspera()
        {
            // Devuelve el número de clientes en la cola.
            return AlmacenDatos.Instancia.ColaAtencion.Count;
        }

    }
}
