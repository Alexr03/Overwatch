using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using System;

namespace Alexr03.Overwatch
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (Overwatch.Instance.Configuration.Instance.DatabasePort == 0) Overwatch.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", Overwatch.Instance.Configuration.Instance.DatabaseAddress, Overwatch.Instance.Configuration.Instance.DatabaseName, Overwatch.Instance.Configuration.Instance.DatabaseUsername, Overwatch.Instance.Configuration.Instance.DatabasePassword, Overwatch.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public string IsBanned(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `banMessage` from `" + Overwatch.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = '" + steamId + "' and (banDuration is null or ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) output = result.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return output;
        }

        public void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + Overwatch.Instance.Configuration.Instance.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + Overwatch.Instance.Configuration.Instance.DatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`admin` varchar(32) NOT NULL,`banMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banDuration` int NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void BanPlayer(string characterName, string steamid, string admin, string banMessage, int duration)
        {
            try
            {

                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (banMessage == null) banMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@banMessage", "Banned by a overwatch agent");
                if (duration == 0)
                {
                    command.Parameters.AddWithValue("@banDuration", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@banDuration", duration);
                }
                command.CommandText = "insert into `" + Overwatch.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@admin,@banMessage,@charactername,now(),@banDuration);";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public class UnbanResult
        {
            public ulong Id;
            public string Name;
        }

        public UnbanResult UnbanPlayer(string player)
        {
            try
            {
                MySqlConnection connection = createConnection();

                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.CommandText = "select steamId,charactername from `" + Overwatch.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player or `charactername` like @player limit 1;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ulong steamId = reader.GetUInt64(0);
                    string charactername = reader.GetString(1);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.CommandText = "delete from `" + Overwatch.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = @steamId;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return new UnbanResult() { Id = steamId, Name = charactername };
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return null;
        }

        public void logItem(string item)
        {
            if (Overwatch.Instance.Configuration.Instance.ShowDebugInfo)
                Logger.Log(item);
        }

        public bool columnExists(string table_name, string table_schema, string column_name)
        {
            bool ret = true;
            MySqlConnection connection = createConnection();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT NULL FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '" + table_name + "' AND table_schema = '" + table_schema + "' AND column_name = '" + column_name + "'";
            connection.Open();
            object test = command.ExecuteScalar();

            if (test == null)
            {
                ret = false;
            }

            connection.Close();
            return ret;
        }

        public int convertTimeToSeconds(string time)
        {
            int ret = 0;
            int tm = 0;
            if (time == null)
                return 0;

            if (time.Contains("d"))
            {
                if (int.TryParse(time.Replace("d", ""), out tm))
                    ret = (tm * 86400);
            }
            else if (time.Contains("h"))
            {
                if (int.TryParse(time.Replace("h", ""), out tm))
                    ret = (tm * 3600);
            }
            else if (time.Contains("m"))
            {
                if (int.TryParse(time.Replace("m", ""), out tm))
                    ret = (tm * 60);
            }
            else if (time.Contains("s"))
            {
                if (int.TryParse(time.Replace("s", ""), out tm))
                    ret = tm;
            }
            else
            {
                if (int.TryParse(time, out tm))
                {
                    //assume seconds, do nothing
                    ret = tm;
                }
                else
                {
                    //they fked it up, return 0;
                    ret = 0;
                }
            }
            return ret;
        }

    }
}