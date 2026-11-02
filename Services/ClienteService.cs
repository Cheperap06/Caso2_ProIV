using System;
using System.Collections.Generic;
using System.Linq;
using Caso2_ProIV.Models;
using Caso2_ProIV.Helpers;

namespace Caso2_ProIV.Services
{
    /// <summary>
    /// Administra las operaciones relacionadas con los clientes.
    /// </summary>
    public class ClienteService
    {
        /// <summary>
        /// Registra un cliente nuevo si sus datos son válidos
        /// y su identificación no está repetida.
        /// </summary>
        public bool RegistrarCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                return false;
            }

            if (!Validador.CampoObligatorio(cliente.Identificacion) ||
                !Validador.CampoObligatorio(cliente.NombreCompleto) ||
                !Validador.CampoObligatorio(cliente.Direccion) ||
                !Validador.CorreoValido(cliente.Correo) ||
                !Validador.TelefonoValido(cliente.Telefono))
            {
                return false;
            }

            if (Validador.IdentificacionDuplicada(cliente.Identificacion))
            {
                return false;
            }

            cliente.Identificacion = cliente.Identificacion.Trim();

            AlmacenDatos.Instancia.Clientes.Add(cliente);

            AlmacenDatos.Instancia.RegistrarOperacion(
                "Cliente registrado: " + cliente.NombreCompleto);

            return true;
        }

        /// <summary>
        /// Consulta un cliente mediante su identificación.
        /// Permite registrar la consulta en el historial
        /// cuando es solicitada directamente por el usuario.
        /// </summary>
        public Cliente ConsultarCliente(
            string identificacion,
            bool registrarEnHistorial = false)
        {
            // Verifica que la identificación no esté vacía.
            if (!Validador.CampoObligatorio(identificacion))
            {
                return null;
            }

            // Busca al cliente utilizando LINQ.
            Cliente cliente = AlmacenDatos.Instancia.Clientes
                .FirstOrDefault(c =>
                    c.Identificacion == identificacion.Trim());

            // Registra la consulta solamente cuando se solicita.
            if (registrarEnHistorial)
            {
                AlmacenDatos.Instancia.RegistrarOperacion(
                    "Consulta ejecutada - Cliente: " +
                    identificacion.Trim());
            }

            return cliente;
        }


        /// <summary>
        /// Devuelve la lista de todos los clientes registrados.
        /// Permite registrar la consulta en el historial.
        /// </summary>
        public List<Cliente> ObtenerClientes(
            bool registrarEnHistorial = false)
        {
            // Obtiene una copia de la lista de clientes.
            List<Cliente> clientes =
                AlmacenDatos.Instancia.Clientes.ToList();

            // Registra la consulta cuando es solicitada.
            if (registrarEnHistorial)
            {
                AlmacenDatos.Instancia.RegistrarOperacion(
                    "Consulta ejecutada - Listado de clientes");
            }

            return clientes;
        }



        /// <summary>
        /// Modifica la información de un cliente existente.
        /// Devuelve true si la modificación fue exitosa.
        /// </summary>
        public bool ModificarCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                return false;
            }

            // Busca al cliente que queremos modificar.
            Cliente clienteExistente = ConsultarCliente(cliente.Identificacion);

            if (clienteExistente == null)
            {
                return false;
            }

            // Valida los nuevos datos del cliente.
            if (!Validador.CampoObligatorio(cliente.NombreCompleto) ||
                !Validador.CampoObligatorio(cliente.Direccion) ||
                !Validador.CorreoValido(cliente.Correo) ||
                !Validador.TelefonoValido(cliente.Telefono))
            {
                return false;
            }

            // Actualiza la información del cliente.
            clienteExistente.NombreCompleto = cliente.NombreCompleto;
            clienteExistente.Telefono = cliente.Telefono;
            clienteExistente.Correo = cliente.Correo;
            clienteExistente.Direccion = cliente.Direccion;
            clienteExistente.Estado = cliente.Estado;

            // Registra la fecha de la última modificación.
            clienteExistente.FechaUltimaModificacion = DateTime.Now;

            // Registra la operación en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Cliente modificado: " + clienteExistente.NombreCompleto);

            return true;
        }

        /// <summary>
        /// Elimina un cliente utilizando su identificación.
        /// Devuelve true si se eliminó correctamente.
        /// </summary>
        public bool EliminarCliente(string identificacion)
        {
            // Busca al cliente que queremos eliminar.
            Cliente cliente = ConsultarCliente(identificacion);

            if (cliente == null)
            {
                return false;
            }

            // Elimina el cliente de la lista.
            AlmacenDatos.Instancia.Clientes.Remove(cliente);

            // Registra la eliminación en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Cliente eliminado: " + cliente.NombreCompleto);

            return true;
        }

        /// <summary>
        /// Cambia el estado de un cliente a Activo o Inactivo.
        /// Devuelve true si el cambio se realizó correctamente.
        /// </summary>
        public bool CambiarEstadoCliente(string identificacion, EstadoCliente nuevoEstado)
        {
            // Busca al cliente por su identificación.
            Cliente cliente = ConsultarCliente(identificacion);

            if (cliente == null)
            {
                return false;
            }

            // Verifica que el estado sea diferente al actual.
            if (cliente.Estado == nuevoEstado)
            {
                return false;
            }

            // Actualiza el estado del cliente.
            cliente.Estado = nuevoEstado;

            // Actualiza la fecha de modificación.
            cliente.FechaUltimaModificacion = DateTime.Now;

            // Registra el cambio en el historial.
            AlmacenDatos.Instancia.RegistrarOperacion(
                "Estado del cliente " + cliente.NombreCompleto +
                " cambiado a " + nuevoEstado);

            return true;
        }

    }
}