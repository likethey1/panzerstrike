using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Web.Security;

namespace ui
{
    public enum UpdateType { ByPlayerID, ByPlayerName } ;
    public class DatabaseManager
    {
        private SqlConnection connection;
        private SqlCommand command;

        private string connectionString;
        public DatabaseManager()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionStringss"].ToString();
            connection = new SqlConnection(connectionString);
        }
        public bool ValidateUser(string username, string password)
        {
            return Membership.ValidateUser(username, password);
        }

        public int UpdatePlayerStats(UpdateType utype, string IDorName, string frags, string deaths, string points, string money)
        {
            string sqlName = "update_Stats_ByPlayerName";
            string sqlID = "update_Stats_ByPlayerID";

            switch (utype)
            {
                case UpdateType.ByPlayerID:
                    {
                        command = new SqlCommand(sqlID, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@playerID", SqlDbType.UniqueIdentifier).Value = IDorName;
                    }
                    break;

                case UpdateType.ByPlayerName:
                    {
                        command = new SqlCommand(sqlName, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@playerName", SqlDbType.UniqueIdentifier).Value = IDorName;
                    }
                    break;
            }


            command.Parameters.Add("@frags", SqlDbType.Int).Value = frags;
            command.Parameters.Add("@deaths", SqlDbType.Int).Value = deaths;
            command.Parameters.Add("@points", SqlDbType.Int).Value = points;
            command.Parameters.Add("@money", SqlDbType.Int).Value = money;

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                return command.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        public string getMap(int mapId)
        {
            string sql = "map_ById";
            command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@mapId", SqlDbType.Int).Value = mapId;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                return command.ExecuteScalar() as string;
            }
            catch (Exception err)
            {
                //throw err;
                return null;
            }

        }

    }
}
