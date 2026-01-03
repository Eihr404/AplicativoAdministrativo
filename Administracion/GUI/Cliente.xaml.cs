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
using Administracion.DP;
using Administracion.MD;

namespace Administracion.GUI
{
    public partial class Cliente : Window
    {
        private readonly ClienteMD clienteMd = new ClienteMD();

        public Cliente()
        {
            InitializeComponent();
            CargarClientes();
        }

        /**
         * Carga la lista completa en el grid
         */
        private void CargarClientes()
        {
            try
            {
                List<ClienteDP> data = clienteMd.ObtenerClientes();
                GridClientes.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar clientes:\n" + ex.Message);
            }
        }

        private ClienteDP? ClienteSeleccionado()
        {
            return GridClientes.SelectedItem as ClienteDP;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string texto = TxtBuscar.Text?.Trim() ?? "";
                GridClientes.ItemsSource = clienteMd.BuscarClientes(texto);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar:\n" + ex.Message);
            }
        }

        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscar.Text = "";
            CargarClientes();
        }

        private void BtnToggleEstado_Click(object sender, RoutedEventArgs e)
        {
            ClienteDP? cli = ClienteSeleccionado();
            if (cli == null)
            {
                MessageBox.Show("Seleccione un cliente.");
                return;
            }

            try
            {
                string nuevoEstado = cli.EstadoCodigo == "A" ? "I" : "A";
                clienteMd.CambiarEstado(cli.UsrNombre, nuevoEstado);
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar estado:\n" + ex.Message);
            }
        }

        private void BtnCambiarRol_Click(object sender, RoutedEventArgs e)
        {
            ClienteDP? cli = ClienteSeleccionado();
            if (cli == null)
            {
                MessageBox.Show("Seleccione un cliente.");
                return;
            }

            // Alterna usuario entre ADMIN y CLIENTE
            string nuevoRol = string.Equals(cli.Rol, "ADMIN", StringComparison.OrdinalIgnoreCase)
                ? "CLIENTE"
                : "ADMIN";

            try
            {
                clienteMd.CambiarRol(cli.UsrNombre, nuevoRol);
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar rol:\n" + ex.Message);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // nada
        }

        // Para el panel de nuevo cliente
        private void CliBtnIngresar(object sender, RoutedEventArgs e)
        {
            PanelNuevoCliente.Visibility = Visibility.Visible;
            LimpiarFormulario();
        }
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            PanelNuevoCliente.Visibility = Visibility.Collapsed;
        }
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtUsrNombre.Text) ||
                    string.IsNullOrWhiteSpace(TxtCedula.Text) ||
                    CmbRol.SelectedItem == null)
                {
                    MessageBox.Show("Complete todos los campos.");
                    return;
                }

                clienteMd.InsertarCliente(
                    TxtUsrNombre.Text.Trim(),
                    TxtCedula.Text.Trim(),
                    TxtPassword.Text.Trim(),
                    ((ComboBoxItem)CmbRol.SelectedItem).Content.ToString()
                );

                PanelNuevoCliente.Visibility = Visibility.Collapsed;
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ingresar cliente:\n" + ex.Message);
            }
        }
        private void LimpiarFormulario()
        {
            TxtUsrNombre.Text = "";
            TxtCedula.Text = "";
            TxtPassword.Text = "";
            CmbRol.SelectedIndex = -1;
        }

    }
}

