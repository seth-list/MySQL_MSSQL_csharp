using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;

using System.Data.SqlClient;
using MySql.Data.MySqlClient;



namespace WpfApplication1
{
    //abstract class for any system implementation
    public abstract class SQL
    {

        public abstract List<string> getAllDataBases();

        public abstract List<string> getAllTables(String database);

        public abstract List<string> getAllColumns(String database, String table);

        public abstract List<List<string>> getDataOfTable(String database, String table);

    }

    public class MySQL : SQL
    {
        private MySqlConnection connection;
        private string server = "localhost";
        private string login = "root";
        private string password = "123";

        private void connect()
        {
            //mysql table
            string myConnectionString = "SERVER=" + server + ";UID='" + login + "';"
                + "PASSWORD='" + password + "';";

            connection = new MySqlConnection(myConnectionString);
            connection.Open();
        }

        private void disconnect()
        {
            connection.Close();
        }


        public override List<string> getAllDataBases()
        {
            connect();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SHOW DATABASES;";
            //========================================================
            MySqlDataReader Reader;
            List<string> data = new List<string>();

            try
            {
                Reader = command.ExecuteReader();

                while (Reader.Read())
                {
                    //добавление баз данных всех
                    data.Add(Reader.GetValue(0).ToString());
                }

                disconnect();
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }


            data.Remove("information_schema");
            data.Remove("performance_schema");
            data.Remove("mysql");


            return data;
        }


        public override List<string> getAllTables(String database)
        {

            connect();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "show tables from " + database + ";";
            //========================================================
            MySqlDataReader Reader;

            List<string> data = new List<string>();

            try
            {
                Reader = command.ExecuteReader();

                while (Reader.Read())
                {
                    data.Add(Reader.GetValue(0).ToString());
                }

                disconnect();
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            return data;

        }


        public override List<string> getAllColumns(String database, String table)
        {

            connect();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = String.Format("SHOW COLUMNS FROM {0} FROM {1}",
                table, database);
            //========================================================
            MySqlDataReader Reader;

            List<string> data = new List<string>();

            try
            {
                Reader = command.ExecuteReader();

                while (Reader.Read())
                {
                    data.Add(Reader.GetValue(0).ToString());
                }

                disconnect();
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            return data;


        }


        public override List<List<string>> getDataOfTable(String database, String table)
        {
            connect();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = String.Format("USE {0}; SELECT * FROM {1};",
                database, table);
            //========================================================
            MySqlDataReader Reader;

            List<List<string>> data = new List<List<string>>();

            try
            {
                Reader = command.ExecuteReader();

                while (Reader.Read())
                {
                    List<string> row = new List<string>();

                    for (int i = 0; i < Reader.FieldCount; i++)
                    {
                        row.Add(Reader.GetValue(i).ToString());
                    }

                    data.Add(row);
                }

                disconnect();
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            return data;
        }
    }

    public class MSSQL : SQL
    {

        private SqlConnection sqlConnection;

        private void connect()
        {
            //using windows authentiphication
            //connetionString="Data Source=ServerName;
            //Initial Catalog=DatabaseName;User ID=UserName;Password=Password"
            var connectionString = string.Format("Data Source=.\\SQLExpress;Integrated Security=SSPI;");

            sqlConnection = new SqlConnection(connectionString);
        }

        public override List<string> getAllDataBases()
        {

            connect();
            DataTable databases = null;

            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    databases = sqlConnection.GetSchema("Databases");
                    sqlConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            List<string> data = new List<string>();
            if (databases != null)
            {
                foreach (DataRow row in databases.Rows)
                {
                    data.Add(row.ItemArray[0].ToString());
                }
            }

            data.Remove("master");
            data.Remove("model");
            data.Remove("msdb");
            data.Remove("tempdb");

            return data;

        }

        public override List<string> getAllTables(String database)
        {

            List<string> data = new List<string>();
            DataTable table = null;

            try
            {
                var connectionString = string.Format("Data Source=.\\SQLExpress;Integrated Security=SSPI;" +
                    "Database=" + database + ";");

                sqlConnection = new SqlConnection(connectionString);

                try
                {
                    using (sqlConnection)
                    {
                        sqlConnection.Open();
                        table = sqlConnection.GetSchema("Tables");
                        sqlConnection.Close();
                    }
                }
                catch (SqlException ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        //2 элемент это база данных
                        data.Add(row.ItemArray[2].ToString());
                    }
                }
                return data;
            }
            catch (InvalidOperationException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            return data;

        }

        public override List<string> getAllColumns(String database, String table)
        {

            List<string> data = new List<string>();

            try
            {
                var connectionString = string.Format("Data Source=.\\SQLExpress;Integrated Security=SSPI;" +
                    "Database=" + database + ";");

                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                try
                {
                    using (sqlConnection)
                    {

                        //Select * From DBNAME.INFORMATION_SCHEMA.COLUMNS
                        SqlCommand command = sqlConnection.CreateCommand();
                        command.CommandText = String.Format("SELECT COLUMN_NAME FROM "
                            + "INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' ", table);
                        //========================================================
                        SqlDataReader Reader;

                        Reader = command.ExecuteReader();

                        while (Reader.Read())
                        {
                            data.Add(Reader.GetValue(0).ToString());
                        }

                        sqlConnection.Close();
                    }
                }
                catch (SqlException ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
            catch (InvalidOperationException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            return data;


        }

        public override List<List<string>> getDataOfTable(String database, String table)
        {

            List<List<string>> data = new List<List<string>>();

            try
            {
                var connectionString = string.Format("Data Source=.\\SQLExpress;Integrated Security=SSPI;" +
                    "Database=" + database + ";");

                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                try
                {
                    using (sqlConnection)
                    {

                        //Select * From DBNAME.INFORMATION_SCHEMA.COLUMNS
                        SqlCommand command = sqlConnection.CreateCommand();
                        command.CommandText = String.Format("SELECT * FROM {0} ", table);
                        //========================================================
                        SqlDataReader Reader;

                        Reader = command.ExecuteReader();

                        while (Reader.Read())
                        {
                            List<string> row = new List<string>();

                            for (int i = 0; i < Reader.FieldCount; i++)
                            {
                                row.Add(Reader.GetValue(i).ToString());
                            }

                            data.Add(row);
                        }

                        sqlConnection.Close();
                    }
                }
                catch (SqlException ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
            catch (InvalidOperationException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            return data;         
        }
    }



}
