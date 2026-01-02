using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;


namespace Administracion.Datos
{
    public static class OracleDB
    {
        /**
         * Crea y devuelve una conexión Oracle usando parámetros leídos desde un archivo .properties.
         */
        public static OracleConnection CrearConexion()
        {
            string propertiesFile = ConfigurationManager.AppSettings["propertiesFile"]
                                    ?? "Conexion.properties";

            var props = LeerProperties(propertiesFile);

            string host = ObtenerValor(props, "db.host");
            string port = ObtenerValor(props, "db.port");
            string serviceName = ObtenerValor(props, "db.serviceName");
            string user = ObtenerValor(props, "db.user");
            string password = ObtenerValor(props, "db.password");

            string dataSource =
                $"(DESCRIPTION=" +
                $"(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))" +
                $"(CONNECT_DATA=(SERVICE_NAME={serviceName})))";

            var connBuilder = new OracleConnectionStringBuilder
            {
                UserID = user,
                Password = password,
                DataSource = dataSource
            };

            return new OracleConnection(connBuilder.ConnectionString);
        }
        /**
         * Lee un archivo .properties (clave=valor) y lo convierte en diccionario.
         */

        private static Dictionary<string, string> LeerProperties(string propertiesPath)
        {
            string finalPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                propertiesPath
            );

            if (!File.Exists(finalPath))
                throw new FileNotFoundException(
                    "No se encontró el archivo: " + finalPath
                );

            var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadAllLines(finalPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                    props[parts[0].Trim()] = parts[1].Trim();
            }

            return props;
        }
        /**
         * Obtiene un valor requerido del diccionario.
         */

        private static string ObtenerValor(Dictionary<string, string> props, string key)
        {
            if (!props.ContainsKey(key))
                throw new Exception($"Falta el parámetro {key} en Conexion.properties");

            return props[key];
        }
    }
}
