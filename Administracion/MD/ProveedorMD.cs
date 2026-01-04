using Oracle.ManagedDataAccess.Client;
using Administracion.Datos;
using Administracion.DP;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Administracion.MD
{
    public class ProveedorMD
    {
        public List<ProveedorDP> ObtenerProveedorMD()
        {
            List<ProveedorDP> lista = new List<ProveedorDP>();
            string query = "SELECT * FROM PROVEEDOR";

            using (OracleConnection conn = OracleDB.CrearConexion())
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                try
                {
                    conn.Open();
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new ProveedorDP
                        {
                            PrvCodigo = reader["PRV_CODIGO"].ToString(),
                            EmpCedulaRuc = reader["EMP_CEDULA_RUC"].ToString(),
                            PrvNombre = reader["PRV_NOMBRE"].ToString(),
                            PrvDireccion = reader["PRV_DIRECCION"].ToString(),
                            PrvTelefono = reader["PRV_TELEFONO"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    // error.general
                    throw new Exception($"{OracleDB.GetConfig("error.general")} (ConsultarAllMD): {ex.Message}");
                }
            }
            return lista;
        }

        public List<ProveedorDP> BuscarProveedorMD(string textoBusqueda)
        {
            var proveedores = new List<ProveedorDP>();

            const string sql = @"
                SELECT PRV_CODIGO, PRV_NOMBRE, PRV_DIRECCION, PRV_TELEFONO 
                FROM PROVEEDOR
                WHERE UPPER(PRV_CODIGO) LIKE UPPER(:pTexto)                   
                ORDER BY PRV_NOMBRE ASC";

            try
            {
                using var conn = OracleDB.CrearConexion();
                conn.Open();

                using var cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add(new OracleParameter("pTexto", $"%{textoBusqueda}%"));

                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    proveedores.Add(new ProveedorDP
                    {
                        PrvCodigo = dr.IsDBNull(0) ? "" : dr.GetString(0),
                        PrvNombre = dr.IsDBNull(1) ? "" : dr.GetString(1),
                        PrvDireccion = dr.IsDBNull(2) ? "" : dr.GetString(2),
                        PrvTelefono = dr.IsDBNull(3) ? "" : dr.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener proveedores (PROVEEDOR).", ex);
            }
            return proveedores;
        }

        public int InsertarProveedorMD(ProveedorDP dp)
        {
            string empCedulaRuc = "1790012345001";
            int filasAfectadas = 0;
            const string sql = @"
                INSERT INTO PROVEEDOR (PRV_CODIGO, EMP_CEDULA_RUC, PRV_NOMBRE, PRV_DIRECCION, PRV_TELEFONO)
                VALUES (:pPrvCodigo, :pEmpCedulaRuc, :pPrvNombre, :pPrvDireccion, :pPrvTelefono)";
            try
            {
                using var conn = OracleDB.CrearConexion();
                conn.Open();
                using var cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add(new OracleParameter("pPrvCodigo", dp.PrvCodigo));
                cmd.Parameters.Add(new OracleParameter("pEmpCedulaRuc", empCedulaRuc));
                cmd.Parameters.Add(new OracleParameter("pPrvNombre", dp.PrvNombre));
                cmd.Parameters.Add(new OracleParameter("pPrvDireccion",dp.PrvDireccion));
                cmd.Parameters.Add(new OracleParameter("pPrvTelefono", dp.PrvTelefono));
                filasAfectadas = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar proveedor (PROVEEDOR).", ex);
            }
            return filasAfectadas;
        }

        public int EliminarProveedorMD(string prvCodigo)
        { 
            string sql  = "DELETE FROM PROVEEDOR WHERE PRV_CODIGO = :cod";

            using (OracleConnection conn = OracleDB.CrearConexion())
            {
                try
                {
                    OracleCommand cmd = new OracleCommand(sql, conn);
                    cmd.Parameters.Add(new OracleParameter("cod", prvCodigo));
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // error.general
                    throw new Exception($"{OracleDB.GetConfig("error.general")} (EliminarMD): {ex.Message}");
                }
            }
        }

        public int ModificarProveedorMD(ProveedorDP dp)
        {
            string sql = @"
                UPDATE PROVEEDOR
                SET PRV_NOMBRE = :pPrvNombre,
                    PRV_DIRECCION = :pPrvDireccion,
                    PRV_TELEFONO = :pPrvTelefono
                WHERE PRV_CODIGO = :pPrvCodigo";

            using (OracleConnection conn = OracleDB.CrearConexion())
            {
                try
                {
                    OracleCommand cmd = new OracleCommand(sql, conn);
                    cmd.Parameters.Add(new OracleParameter("pPrvNombre", dp.PrvNombre));
                    cmd.Parameters.Add(new OracleParameter("pPrvDireccion", dp.PrvDireccion));
                    cmd.Parameters.Add(new OracleParameter("pPrvTelefono", dp.PrvTelefono));

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    throw new Exception(Properties.Resources.error_general + " (ModificarProveedor): " + ex.Message);
                }
            }

        }

    }
}
