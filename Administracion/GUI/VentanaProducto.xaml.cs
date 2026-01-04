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
                // error.general
                MessageBox.Show($"{OracleDB.GetConfig("error.general")} {ex.Message}");
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
                // error.general
                MessageBox.Show($"{OracleDB.GetConfig("error.general")} {ex.Message}");
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
                    productoDP.Codigo = criterio;
                    var resultado = productoDP.ConsultarByCodDP();

                    if (resultado == null || resultado.Count == 0)
                    {
                        // error.no_encontrado
                        MessageBox.Show(OracleDB.GetConfig("error.no_encontrado"));
                    }
                    prdDatGri.ItemsSource = resultado;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{OracleDB.GetConfig("error.general")} {ex.Message}");
            }
        }

        private void prdBtnIngresar_Click(object sender, RoutedEventArgs e)
        {
            esModificacion = false;
            // titulo.formulario.nuevo
            lblTituloForm.Text = OracleDB.GetConfig("titulo.formulario.nuevo");
            LimpiarCampos();
            txtPrdCodigo.IsEnabled = true;
            PanelFormularioPrd.Visibility = Visibility.Visible;
        }

        private void prdBtnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (prdDatGri.SelectedItem is not ProductoDP seleccionado)
            {
                // error.no_encontrado (como aviso de selección)
                MessageBox.Show(OracleDB.GetConfig("error.no_encontrado"));
                return;
            }

            esModificacion = true;
            productoDP = seleccionado;
            // titulo.formulario.editar
            lblTituloForm.Text = OracleDB.GetConfig("titulo.formulario.editar");

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
                    // error.validacion
                    MessageBox.Show(OracleDB.GetConfig("error.validacion"));
                    return;
                }

                // Validación de formato numérico
                if (!double.TryParse(txtPrdPrecio.Text, out double precio))
                {
                    // error.formato.numerico
                    MessageBox.Show(OracleDB.GetConfig("error.formato.numerico"));
                    return;
                }

                // mensaje.confirmacion.guardar
                MessageBoxResult confirmar = MessageBox.Show(
                    OracleDB.GetConfig("mensaje.confirmacion.guardar"),
                    OracleDB.GetConfig("titulo.confirmacion"),
                    MessageBoxButton.YesNo);

                if (confirmar == MessageBoxResult.No) return;

                ProductoDP datos = new ProductoDP
                {
                    Codigo = txtPrdCodigo.Text.Trim(),
                    Nombre = txtPrdNombre.Text.Trim(),
                    Descripcion = txtPrdDesc.Text.Trim(),
                    PrecioVenta = precio,
                    Utilidad = double.Parse(txtPrdUtilidad.Text),
                    AltTextImagen = txtPrdAltImagen.Text.Trim(),
                    CategoriaCodigo = cmbCategoria.SelectedValue.ToString(),
                    ClasificacionCodigo = cmbClasificacion.SelectedValue.ToString(),
                    UnidadMedidaCodigo = cmbUnidad.SelectedValue.ToString(),
                    PrecioVentaAnt = esModificacion ? productoDP.PrecioVenta : 0
                };

                bool resultado = esModificacion ? datos.ModificarDP() : datos.IngresarDP();

                if (resultado)
                {
                    // exito.guardar / exito.actualizar
                    string msgExito = esModificacion ?
                        OracleDB.GetConfig("exito.actualizar") :
                        OracleDB.GetConfig("exito.guardar");

                    MessageBox.Show(msgExito);
                    PanelFormularioPrd.Visibility = Visibility.Collapsed;
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{OracleDB.GetConfig("error.general")} {ex.Message}");
            }
        }

        private void prdBtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (prdDatGri.SelectedItem is not ProductoDP seleccionado) return;

            // mensaje.confirmacion.borrar
            MessageBoxResult confirmar = MessageBox.Show(
                OracleDB.GetConfig("mensaje.confirmacion.borrar"),
                OracleDB.GetConfig("titulo.confirmacion"),
                MessageBoxButton.YesNo);

            if (confirmar == MessageBoxResult.Yes)
            {
                if (seleccionado.EliminarDP())
                {
                    // exito.eliminar
                    MessageBox.Show(OracleDB.GetConfig("exito.eliminar"));
                    CargarProductos();
                }
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
            return string.IsNullOrWhiteSpace(txtPrdCodigo.Text) ||
                   cmbCategoria.SelectedValue == null ||
                   string.IsNullOrWhiteSpace(txtPrdNombre.Text);
        }
    }
}