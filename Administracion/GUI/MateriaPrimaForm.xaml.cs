using Administracion.DP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Administracion.GUI
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class MateriaPrimaForm : Window
    {
        public MateriaPrimaDP Resultado { get; set; }
        private bool esModificacion = false;

        // Constructor para INSERTAR
        public MateriaPrimaForm()
        {
            InitializeComponent();
            this.Title = "Nuevo Registro de Materia Prima";
        }

        // Constructor para MODIFICAR
        public MateriaPrimaForm(MateriaPrimaDP datosExistentes) : this()
        {
            esModificacion = true;
            this.Title = "Modificar Materia Prima";

            // Llenamos los campos con los datos existentes
            txtCodigo.Text = datosExistentes.MtpCodigo;
            txtCodigo.IsEnabled = false; // Llave primaria no editable

            txtUnidad.Text = datosExistentes.UmeCodigo;
            txtNombre.Text = datosExistentes.MtpNombre;
            txtDescripcion.Text = datosExistentes.MtpDescripcion;

            // Mostramos los precios (usando el actual)
            txtPrecio.Text = datosExistentes.MtpPrecioCompra.ToString();

            // Guardamos el precio anterior internamente por si el MD lo necesita
            Resultado = datosExistentes;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validación básica
                if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("Por favor, complete los campos obligatorios (Código y Nombre).");
                    return;
                }

                // Si es modificación, el precio actual pasa a ser el anterior
                double precioAnterior = 0;
                if (esModificacion && Resultado != null)
                {
                    precioAnterior = Resultado.MtpPrecioCompra;
                }

                Resultado = new MateriaPrimaDP
                {
                    MtpCodigo = txtCodigo.Text.Trim(),
                    UmeCodigo = txtUnidad.Text.Trim(),
                    MtpNombre = txtNombre.Text.Trim(),
                    MtpDescripcion = txtDescripcion.Text.Trim(),
                    MtpPrecioCompraAnt = precioAnterior,
                    MtpPrecioCompra = double.Parse(txtPrecio.Text)
                };

                this.DialogResult = true;
            }
            catch (FormatException)
            {
                MessageBox.Show("El precio debe ser un valor numérico válido.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
