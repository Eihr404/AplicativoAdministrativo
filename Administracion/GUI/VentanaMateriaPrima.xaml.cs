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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Administracion.DP; // Donde están tus Getters y Setters
using Administracion.MD; // Donde está tu conexión a Oracle

namespace Administracion.GUI
{
    public partial class MateriaPrima : UserControl
    {
        // Instanciamos el Modelo para poder usar sus métodos de base de datos
        MateriaPrimaMD modelo = new MateriaPrimaMD();

        public MateriaPrima()
        {
            InitializeComponent();
            // Llamamos al método de consulta apenas se crea la instancia
            CargarDatosIniciales();
        }

        private void CargarDatosIniciales()
        {
            try
            {
                // 1. Instanciamos el DP como mensajero
                MateriaPrimaDP mensajeroDP = new MateriaPrimaDP();

                // 2. Llamamos al método especializado en traer todo
                // Ya no dependemos de si el TextBox está vacío o no
                var resultado = mensajeroDP.ConsultaGeneralDP();

                // 3. Asignamos al DataGrid
                mtpDatGri.ItemsSource = resultado;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos iniciales: " + ex.Message);
            }
        }
        private void ProbarConexion()
        {
            string cs = "User Id=a_prueba;Password=lticPUCE24;Data Source=192.168.5.125:1521/PRUEBA";
            using (var conn = new Oracle.ManagedDataAccess.Client.OracleConnection(cs))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("¡Conexión exitosa a Oracle!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fallo de conexión: " + ex.Message);
                }
            }
        }

        // Evento para el botón CONSULTAR
        private void mtpBtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MateriaPrimaDP mensajeroDP = new MateriaPrimaDP();
                string codigoABuscar = mtpTxtblBuscarCodigo.Text.Trim();
                List<MateriaPrimaDP> resultado;

                // Decidimos qué función usar según el TextBox
                if (string.IsNullOrEmpty(codigoABuscar))
                {
                    resultado = mensajeroDP.ConsultaGeneralDP();
                }
                else
                {
                    resultado = mensajeroDP.ConsultaPorParametroDP(codigoABuscar);
                }

                mtpDatGri.ItemsSource = resultado;

                if (resultado.Count == 0 && !string.IsNullOrEmpty(codigoABuscar))
                {
                    MessageBox.Show("No se encontró ninguna materia prima con el código: " + codigoABuscar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar: " + ex.Message);
            }
        }

        // Evento para el botón INGRESAR
        private void mtpBtnIngresar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Abrimos el formulario de ingreso
            MateriaPrimaForm formulario = new MateriaPrimaForm();
            formulario.Owner = Window.GetWindow(this);

            // 2. Si el usuario presionó "Guardar" en el formulario
            if (formulario.ShowDialog() == true)
            {
                try
                {
                    // 3. Obtenemos el objeto DP del formulario
                    MateriaPrimaDP nuevoRegistro = formulario.Resultado;

                    // 4. EL CAMBIO CLAVE: El DP llama a su propio método de inserción
                    int filas = nuevoRegistro.InsertarDP();

                    if (filas > 0)
                    {
                        MessageBox.Show("Materia prima registrada correctamente.");
                        mtpBtnConsultar_Click(null, null); // Refrescar la tabla
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar: " + ex.Message);
                }
            }
        }

        // Evento para el botón Modificar
        private void mtpBtnModificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MateriaPrimaDP mensajeroDP = new MateriaPrimaDP();
                MateriaPrimaDP seleccionado = null;

                // 1. Obtener el objeto a modificar
                if (mtpDatGri.SelectedItem is MateriaPrimaDP filaSeleccionada)
                {
                    seleccionado = filaSeleccionada;
                }
                else if (!string.IsNullOrEmpty(mtpTxtblBuscarCodigo.Text.Trim()))
                {
                    // El GUI le pide al DP que busque la información
                    mensajeroDP.MtpCodigo = mtpTxtblBuscarCodigo.Text.Trim();
                    var resultados = mensajeroDP.ConsultaGeneralDP(); // Método en DP que llama al MD

                    if (resultados.Count > 0)
                        seleccionado = resultados[0];
                }

                if (seleccionado == null)
                {
                    MessageBox.Show("Por favor, seleccione una fila o ingrese un código válido.");
                    return;
                }

                // 2. Abrir el formulario con los datos cargados
                MateriaPrimaForm formulario = new MateriaPrimaForm(seleccionado);
                formulario.Owner = Window.GetWindow(this);

                if (formulario.ShowDialog() == true)
                {
                    // 3. El objeto Resultado del formulario (que es un DP) ejecuta la actualización
                    int filas = formulario.Resultado.ActualizarDP();

                    if (filas > 0)
                    {
                        MessageBox.Show("Registro actualizado correctamente.");
                        mtpBtnConsultar_Click(null, null); // Refrescar tabla
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar: " + ex.Message);
            }
        }

        // Evento para el botón ELIMINAR
        private void mtpBtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Creamos el objeto DP que actuará como mensajero
                MateriaPrimaDP mensajeroDP = new MateriaPrimaDP();
                string nombreMostrar = "";

                // 2. Prioridad: Revisar si hay una fila seleccionada
                if (mtpDatGri.SelectedItem is MateriaPrimaDP seleccionado)
                {
                    mensajeroDP.MtpCodigo = seleccionado.MtpCodigo;
                    nombreMostrar = seleccionado.MtpNombre;
                }
                // 3. Si no hay selección, revisar el TextBox
                else if (!string.IsNullOrEmpty(mtpTxtblBuscarCodigo.Text.Trim()))
                {
                    mensajeroDP.MtpCodigo = mtpTxtblBuscarCodigo.Text.Trim();
                    nombreMostrar = "el registro con código " + mensajeroDP.MtpCodigo;
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione una fila o ingrese un código.");
                    return;
                }

                // 4. Confirmación del usuario
                var respuesta = MessageBox.Show($"¿Está seguro de que desea eliminar {nombreMostrar}?",
                                               "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (respuesta == MessageBoxResult.Yes)
                {
                    // LLAMADA AL DP (Él se encarga de hablar con el MD)
                    int filasAfectadas = mensajeroDP.EliminarDP();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Registro eliminado correctamente.");
                        mtpTxtblBuscarCodigo.Clear();
                        mtpBtnConsultar_Click(null, null); // Refrescar DataGrid
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el registro en la base de datos.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
