using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Administracion.MD;
namespace Administracion.DP
{
    public class ProveedorDP
    {
        public string PrvCodigo { get; set; }          // PRV_CODIGO
        public string EmpCedulaRuc { get; set; }       // EMP_CEDULA_RUC
        public string PrvNombre { get; set; }          // PRV_NOMBRE
        public string PrvTelefono { get; set; }        // PRV_TELEFONO
        public string PrvDireccion { get; set; }       // PRV_DIRECCION


        private ProveedorMD prvProveedorMD = new ProveedorMD();

        public List<ProveedorDP> ObtenerProveedoresDP()
        {            
            ProveedorMD prvProveedorMD = new ProveedorMD();
            return prvProveedorMD.ObtenerProveedorMD();
        }

        public List<ProveedorDP> BuscarProveedorDP(string codigoPrv)
        {
            ProveedorMD prvProveedorMD = new ProveedorMD();
            return prvProveedorMD.BuscarProveedorMD(codigoPrv);
        }

        public int InsertarProveedorDP() 
        {
            return prvProveedorMD.InsertarProveedorMD(this);
        }

        public int ActualizarProveedorDP()
        {
            return prvProveedorMD.ModificarProveedorMD(this);
        }

        public int EliminarProveedorDP()
        {
            return prvProveedorMD.EliminarProveedorMD(this.PrvCodigo);
        }

    }
}
