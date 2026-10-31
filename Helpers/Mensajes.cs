using System.Windows.Forms;

namespace Caso2_ProIV.Helpers
{
    /// <summary>
    /// Todo lo visual de las validaciones: ErrorProvider y MessageBox.
    /// Así los formularios no repiten el mismo código en cada botón.
    /// </summary>
    public static class Mensajes
    {
        private const string Titulo = "ElectroHogar S.A.";

        // ---------- ErrorProvider ----------

        /// <summary>
        /// Muestra u oculta el icono de error junto al control.
        /// Devuelve esValido, para poder encadenar: ok &amp;= Mensajes.ValidarCampo(...)
        /// </summary>
        public static bool ValidarCampo(ErrorProvider errorProvider, Control control,
                                        bool esValido, string mensajeError)
        {
            errorProvider.SetError(control, esValido ? string.Empty : mensajeError);
            return esValido;
        }

        public static void LimpiarError(ErrorProvider errorProvider, Control control)
        {
            errorProvider.SetError(control, string.Empty);
        }

        /// <summary>Quita los errores de todos los controles de un contenedor (Form, GroupBox, Panel).</summary>
        public static void LimpiarErrores(ErrorProvider errorProvider, Control contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                errorProvider.SetError(control, string.Empty);

                if (control.HasChildren)
                {
                    LimpiarErrores(errorProvider, control);
                }
            }
        }

        // ---------- MessageBox ----------

        public static void Informacion(string mensaje)
        {
            MessageBox.Show(mensaje, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Advertencia(string mensaje)
        {
            MessageBox.Show(mensaje, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Error(string mensaje)
        {
            MessageBox.Show(mensaje, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>Pregunta Sí/No. Devuelve true si el usuario presiona Sí.</summary>
        public static bool Confirmar(string pregunta)
        {
            return MessageBox.Show(pregunta, Titulo, MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
