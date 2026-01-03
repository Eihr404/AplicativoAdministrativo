using Administracion.DP;
using Administracion.MD;
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
    /// Lógica de interacción para ProductoForm.xaml
    /// </summary>
    public partial class ProductoForm : Window
    {
        public ProductoDP productoDP { get; set; }
        private bool esModificacion = false;
        public ProductoForm()
        {
            InitializeComponent();
            CargarCombos();
            this.Title = "Nuevo Registro de Producto";
        }
        /* Carga los datos de los combobox para categoria, clasificación y unidad de medida */
        private void CargarCombos()
        {
            prdComBCategoria.ItemsSource = productoDP.ObtenerCategoriasDP();
            prdComBClasificacion.ItemsSource = productoDP.ObtenerClasificacionesDP();
            prdComBUnidadM.ItemsSource = productoDP.ObtenerUnidadesMedidaDP();
        }
        private void Combo_MostrarDescripcion(object sender, SelectionChangedEventArgs e) 
        { 
            if (sender is ComboBox combo && combo.SelectedItem is string item) 
            { 
                string descripcion = item.Split('|')[1]; 
                combo.Text = descripcion; 
            } 
        }
        /* Actualiza visualmente la combobox en base al código seleccionado, mostrando la descripción */
        private void SeleccionarItemPorCodigo(ComboBox combo, string codigo)
        {
            foreach (string item in combo.Items)
            {
                if (item.StartsWith(codigo + "|"))
                {
                    combo.SelectedItem = item;
                    break;
                }
            }
        }
        /* Obtener el codigo de los combobox y no la descripción */
        private string ObtenerCodigoDesdeCombo(ComboBox combo)
        {
            if (combo.SelectedItem is string item)
                return item.Split('|')[0];

            return string.Empty;
        }

        public ProductoForm(ProductoDP datosExistentes) : this()
        {
            esModificacion = true;
            this.Title = "Modificar Producto";

            prdTxtBCodigo.Text = datosExistentes.Codigo;
            prdTxtBCodigo.IsEnabled = false;

            SeleccionarItemPorCodigo(prdComBCategoria, datosExistentes.CategoriaCodigo);
            SeleccionarItemPorCodigo(prdComBClasificacion, datosExistentes.ClasificacionCodigo);
            SeleccionarItemPorCodigo(prdComBUnidadM, datosExistentes.UnidadMedidaCodigo);

            prdTxtBNombre.Text = datosExistentes.Nombre;
            prdTxtBDescripcion.Text = datosExistentes.Descripcion;
            prdTxtBPrecioVent.Text = datosExistentes.PrecioVenta.ToString();
            prdTxtBUtilidad.Text = datosExistentes.Utilidad.ToString();
            prdTxtBImagen.Text = datosExistentes.Imagen;
            prdTxtBAltImagen.Text = datosExistentes.AltTextImagen;

            productoDP = datosExistentes;
        }

        private void prdBtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validación: ningún campo vacío
                if (CamposInvalidos())
                {
                    MessageBox.Show("Todos los campos son obligatorios.");
                    return;
                }

                double precioVenta = double.Parse(prdTxtBPrecioVent.Text);
                double utilidad = double.Parse(prdTxtBUtilidad.Text);

                // Precio anterior solo si es modificación
                double precioAnterior = 0;
                if (esModificacion && productoDP != null)
                {
                    precioAnterior = productoDP.PrecioVenta;
                }

                productoDP = new ProductoDP
                {
                    Codigo = prdTxtBCodigo.Text.Trim(),
                    CategoriaCodigo = ObtenerCodigoDesdeCombo(prdComBCategoria),
                    ClasificacionCodigo = ObtenerCodigoDesdeCombo(prdComBClasificacion),
                    UnidadMedidaCodigo = ObtenerCodigoDesdeCombo(prdComBUnidadM),
                    Nombre = prdTxtBNombre.Text.Trim(),
                    Descripcion = prdTxtBDescripcion.Text.Trim(),
                    PrecioVentaAnt = precioAnterior,
                    PrecioVenta = precioVenta,
                    Utilidad = utilidad,
                    Imagen = prdTxtBImagen.Text.Trim(),
                    AltTextImagen = prdTxtBAltImagen.Text.Trim()
                };

                this.DialogResult = true;
            }
            catch (FormatException)
            {
                MessageBox.Show("Precio y utilidad deben ser valores numéricos válidos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void prdBtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool CamposInvalidos()
        {
            return string.IsNullOrWhiteSpace(prdTxtBCodigo.Text)
                || string.IsNullOrWhiteSpace(prdComBCategoria.Text)
                || string.IsNullOrWhiteSpace(prdComBClasificacion.Text)
                || string.IsNullOrWhiteSpace(prdComBUnidadM.Text)
                || string.IsNullOrWhiteSpace(prdTxtBNombre.Text)
                || string.IsNullOrWhiteSpace(prdTxtBDescripcion.Text)
                || string.IsNullOrWhiteSpace(prdTxtBPrecioVent.Text)
                || string.IsNullOrWhiteSpace(prdTxtBUtilidad.Text)
                || string.IsNullOrWhiteSpace(prdTxtBImagen.Text)
                || string.IsNullOrWhiteSpace(prdTxtBAltImagen.Text);
        }
    }
}
