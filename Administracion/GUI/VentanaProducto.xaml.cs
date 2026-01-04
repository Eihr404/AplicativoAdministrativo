using Administracion.Datos;
using Administracion.DP;
using Administracion.MD;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Administracion.GUI
{
    public partial class VentanaProducto : UserControl
    {
        private ProductoDP productoDP;
        private bool esModificacion = false;

        public VentanaProducto()
        {
            InitializeComponent();
            productoDP = new ProductoDP();
            CargarProductos();
            CargarCombos();
        }

        private void CargarProductos()
        {
            try
            {
                prdDatGri.ItemsSource = productoDP.ConsultarAllDP();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void CargarCombos()
        {
            try
            {
                cmbCategoria.ItemsSource = new CategoriaDP().ConsultarTodos();
                cmbClasificacion.ItemsSource = new ClasificacionDP().ConsultarTodos();
                cmbUnidad.ItemsSource = new UnidadMedidaDP().ConsultarTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en catálogos: " + ex.Message);
            }
        }

        private void prdBtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string criterio = prdTxtblBuscarCodigo.Text.Trim();
                if (string.IsNullOrEmpty(criterio))
                {
                    CargarProductos();
                }
                else
                {
                    // PASO CLAVE: Asignar el criterio al objeto que hará la consulta
                    productoDP.Codigo = criterio;

                    // Ahora la propiedad 'this.Codigo' en el DP tendrá el valor correcto
                    prdDatGri.ItemsSource = productoDP.ConsultarByCodDP();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar: " + ex.Message);
            }
        }

        private void prdBtnIngresar_Click(object sender, RoutedEventArgs e)
        {
            esModificacion = false;
            lblTituloForm.Text = "NUEVO REGISTRO DE PRODUCTO";
            LimpiarCampos();
            txtPrdCodigo.IsEnabled = true;
            PanelFormularioPrd.Visibility = Visibility.Visible;
        }

        private void prdBtnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (prdDatGri.SelectedItem is not ProductoDP seleccionado)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            esModificacion = true;
            productoDP = seleccionado; // Guardamos el estado actual para el PrecioVentaAnt
            lblTituloForm.Text = "MODIFICAR PRODUCTO";

            txtPrdCodigo.Text = seleccionado.Codigo;
            txtPrdCodigo.IsEnabled = false;
            txtPrdNombre.Text = seleccionado.Nombre;
            txtPrdDesc.Text = seleccionado.Descripcion;
            txtPrdPrecio.Text = seleccionado.PrecioVenta.ToString();
            txtPrdUtilidad.Text = seleccionado.Utilidad.ToString();
            txtPrdAltImagen.Text = seleccionado.AltTextImagen;

            cmbCategoria.SelectedValue = seleccionado.CategoriaCodigo;
            cmbClasificacion.SelectedValue = seleccionado.ClasificacionCodigo;
            cmbUnidad.SelectedValue = seleccionado.UnidadMedidaCodigo;

            PanelFormularioPrd.Visibility = Visibility.Visible;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CamposInvalidos())
                {
                    MessageBox.Show("Todos los campos son obligatorios.");
                    return;
                }

                // Creamos el objeto con los datos del formulario
                ProductoDP datos = new ProductoDP
                {
                    Codigo = txtPrdCodigo.Text.Trim(),
                    Nombre = txtPrdNombre.Text.Trim(),
                    Descripcion = txtPrdDesc.Text.Trim(),
                    PrecioVenta = double.Parse(txtPrdPrecio.Text),
                    Utilidad = double.Parse(txtPrdUtilidad.Text),
                    AltTextImagen = txtPrdAltImagen.Text.Trim(),
                    CategoriaCodigo = cmbCategoria.SelectedValue.ToString(),
                    ClasificacionCodigo = cmbClasificacion.SelectedValue.ToString(),
                    UnidadMedidaCodigo = cmbUnidad.SelectedValue.ToString(),
                    // Si es modificación, el anterior es el que ya tenía el objeto productoDP
                    PrecioVentaAnt = esModificacion ? productoDP.PrecioVenta : 0
                };

                bool resultado = esModificacion ? datos.ModificarDP() : datos.IngresarDP();

                if (resultado)
                {
                    MessageBox.Show("Operación exitosa.");
                    PanelFormularioPrd.Visibility = Visibility.Collapsed;
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void prdBtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (prdDatGri.SelectedItem is not ProductoDP seleccionado) return;

            if (MessageBox.Show($"¿Eliminar {seleccionado.Nombre}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (seleccionado.EliminarDP()) CargarProductos();
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            PanelFormularioPrd.Visibility = Visibility.Collapsed;
        }

        private void LimpiarCampos()
        {
            txtPrdCodigo.Text = "";
            txtPrdNombre.Text = "";
            txtPrdDesc.Text = "";
            txtPrdPrecio.Text = "";
            txtPrdUtilidad.Text = "";
            txtPrdAltImagen.Text = "";
            cmbCategoria.SelectedIndex = -1;
            cmbClasificacion.SelectedIndex = -1;
            cmbUnidad.SelectedIndex = -1;
        }

        private bool CamposInvalidos()
        {
            return string.IsNullOrWhiteSpace(txtPrdCodigo.Text) || cmbCategoria.SelectedValue == null;
        }
    }
}