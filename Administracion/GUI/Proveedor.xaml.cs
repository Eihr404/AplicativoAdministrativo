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
using Administracion.Datos;
using System.ComponentModel;

namespace Administracion.GUI
{
    /// <summary>
    /// Lógica de interacción para Proveedor.xaml
    /// </summary>
    public partial class Proveedor : UserControl
    {
        public Proveedor()
        {
            InitializeComponent();
            PrvGuiCargarDatosIniciales();
        }

        private void PrvGuiCargarDatosIniciales()
        {
            try
            {
                ProveedorDP proveedoresDP = new ProveedorDP();
                GridProveedor.ItemsSource = proveedoresDP.ObtenerProveedoresDP();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{OracleDB.GetConfig("error.general")} {ex.Message}");
            }
        }



        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CliBtnIngresar(object sender, RoutedEventArgs e)
        {
            PanelNuevoProveedor.Visibility = Visibility.Visible;
        }

        private void BtnToggleEstado_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnModificarRol_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
