using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Administracion.MD;

namespace Administracion.DP
{
    public class UnidadMedidaDP
    {
        public string UmeCodigo { get; set; }
        public string UmeDescripcion { get; set; }

        public List<UnidadMedidaDP> ConsultarTodos()
        {
            return new UnidadMedidaMD().ConsultarTodosMD();
        }
    }
}
