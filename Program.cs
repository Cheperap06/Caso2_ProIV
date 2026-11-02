using Caso2_ProIV.Forms;
using Caso2_ProIV.Models;
using Caso2_ProIV.Services;
using System;
using System.Windows.Forms;

namespace Caso2_ProIV

{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Crear el servicio de usuarios.
            UsuarioService usuarioService = new UsuarioService();

            // Cargar las cuentas de ejemplo del administrador
            // y del empleado al iniciar la aplicación.
            usuarioService.CargarUsuariosIniciales();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
