namespace Caso2_ProIV.Models
{
    /// <summary>Estado de un cliente dentro del sistema.</summary>
    public enum EstadoCliente
    {
        Activo,
        Inactivo
    }

    /// <summary>Estado de un producto según su existencia en inventario.</summary>
    public enum EstadoProducto
    {
        Disponible,
        Agotado
    }

    /// <summary>
    /// Define los roles de acceso de los usuarios del sistema.
    /// </summary>
    public enum RolUsuario
    {
        Administrador,
        Empleado
    }
}
