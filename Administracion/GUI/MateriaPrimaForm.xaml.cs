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

            // 1. Cargamos el ComboBox al abrir la ventana
            CargarUnidades();
        }

        // Constructor para MODIFICAR
        public MateriaPrimaForm(MateriaPrimaDP datosExistentes) : this()
        {
            esModificacion = true;
            this.Title = "Modificar Materia Prima";

            txtCodigo.Text = datosExistentes.MtpCodigo;
            txtCodigo.IsEnabled = false;

            txtNombre.Text = datosExistentes.MtpNombre;
            txtDescripcion.Text = datosExistentes.MtpDescripcion;
            txtPrecio.Text = datosExistentes.MtpPrecioCompra.ToString();

            // 2. Seleccionar el ítem en el ComboBox usando el valor que viene de la BD
            cmbUnidadMedida.SelectedValue = datosExistentes.UmeCodigo;

            Resultado = datosExistentes;
        }

        private void CargarUnidades()
        {
            try
            {
                UnidadMedidaDP unidad = new UnidadMedidaDP();
                // Llama al método que creamos en el DP para traer los códigos de Oracle
                cmbUnidadMedida.ItemsSource = unidad.ConsultarTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar unidades de medida: " + ex.Message);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 3. Validación: Incluimos el ComboBox en la validación obligatoria
                if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                    string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    cmbUnidadMedida.SelectedValue == null)
                {
                    MessageBox.Show("Por favor, complete Código, Nombre y seleccione una Unidad de Medida.");
                    return;
                }

                double precioAnterior = 0;
                if (esModificacion && Resultado != null)
                {
                    precioAnterior = Resultado.MtpPrecioCompra;
                }

                // 4. Creamos el objeto Resultado capturando los datos de la interfaz
                Resultado = new MateriaPrimaDP
                {
                    MtpCodigo = txtCodigo.Text.Trim(),
                    // USAMOS el valor seleccionado del ComboBox (SelectedValue)
                    UmeCodigo = cmbUnidadMedida.SelectedValue.ToString(),
                    MtpNombre = txtNombre.Text.Trim(),
                    MtpDescripcion = txtDescripcion.Text.Trim(),
                    MtpPrecioCompraAnt = precioAnterior,
                    MtpPrecioCompra = double.Parse(txtPrecio.Text)
                };

                this.DialogResult = true;
            }
            catch (FormatException)
            {
                MessageBox.Show("El precio debe ser un valor numérico válido (use coma o punto según su región).");
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
