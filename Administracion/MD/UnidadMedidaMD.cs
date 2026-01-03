using Administracion.Datos;
using Administracion.DP;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Administracion.MD
{
    public class UnidadMedidaMD
    {
        public List<UnidadMedidaDP> ConsultarTodosMD()
        {
            List<UnidadMedidaDP> lista = new List<UnidadMedidaDP>();

            // Usamos la conexión robusta de tu compañero
            using (OracleConnection conn = OracleDB.CrearConexion())
            {
                string sql = "SELECT UME_CODIGO, UME_DESCRIPCION FROM UNIDAD_MEDIDA ORDER BY UME_CODIGO ASC";
                OracleCommand cmd = new OracleCommand(sql, conn);

                try
                {
                    conn.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new UnidadMedidaDP
                            {
                                UmeCodigo = reader["UME_CODIGO"].ToString(),
                                UmeDescripcion = reader["UME_DESCRIPCION"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en UnidadMedidaMD: " + ex.Message);
                }
            }
            return lista;
        }
    }
}
