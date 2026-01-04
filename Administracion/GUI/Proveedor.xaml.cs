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
    /// Lógica de interacción para Proveedor.xaml
    /// </summary>
    public partial class Proveedor : UserControl
    {
        public Proveedor()
        {
            InitializeComponent();
            CargarProveedores();
        }

        private void CargarProveedores()
        {
            try
            {
                List<ProveedorDP> lista = ProveedorDP.Listar();
                dgProveedores.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar proveedores:\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
