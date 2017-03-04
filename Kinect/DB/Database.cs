using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mime;
using System.Windows;
using System.Windows.Documents;

namespace Kinect.DB
{
    public class Database: IDisposable
    {
        private string strConnectionString;
        private DbConnection objConnection;
        private DbCommand objCommand;
        private DbProviderFactory objFactory = null;
        private bool boolHandleErrors;
        private string strLastError;
        private bool boolLogError;
        private string strLogFile;

        public Database(string connectionstring, Providers provider)
        {
            strConnectionString = connectionstring;
            switch (provider)
            {
                case Providers.SqlServer:
                    objFactory = SqlClientFactory.Instance;
                    break;
                case Providers.OleDb:
                    objFactory = OleDbFactory.Instance;
                    break;
                case Providers.ODBC:
                    objFactory = OdbcFactory.Instance;
                    break;
                case Providers.ConfigDefined:
                    string providername = ConfigurationManager.ConnectionStrings["DefaultConnection"].ProviderName;
                    switch (providername)
                    {
                        case "System.Data.SqlClient":
                            objFactory = SqlClientFactory.Instance;
                            break;
                        case "System.Data.OleDb":
                            objFactory = OleDbFactory.Instance;
                            break;
                        case "System.Data.Odbc":
                            objFactory = OdbcFactory.Instance;
                            break;
                        default:
                            objFactory = SqlClientFactory.Instance;
                            break;
                    }
                    break;
            }
            objConnection = objFactory.CreateConnection();
            objCommand = objFactory.CreateCommand();
            objConnection.ConnectionString = strConnectionString;
            objCommand.Connection = objConnection;
        }

        public Database(Providers provider)
            : this(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, provider)
        {
        }

        public Database(string connectionstring)
            : this(connectionstring, Providers.SqlServer)
        {
        }

        public Database()
            : this(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.Replace("{0}", System.AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug", "Data")), Providers.SqlServer)
        {
        }

        public bool HandleErrors
        {
            get { return boolHandleErrors; }
            set { boolHandleErrors = value; }
        }

        public string LastError
        {
            get { return strLastError; }
        }

        public bool LogErrors
        {
            get { return boolLogError; }
            set { boolLogError = value; }
        }

        public string LogFile
        {
            get { return strLogFile; }
            set { strLogFile = value; }
        }

        public int AddParameter(string name, object value)
        {
            DbParameter p = objFactory.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            return objCommand.Parameters.Add(p);
        }

        public int AddParameter(DbParameter parameter)
        {
            return objCommand.Parameters.Add(parameter);
        }

        public DbCommand Command
        {
            get { return objCommand; }
        }

        public void BeginTransaction()
        {
            if (objConnection.State == System.Data.ConnectionState.Closed)
            {
                objConnection.Open();
            }
            objCommand.Transaction = objConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            objCommand.Transaction.Commit();
            objConnection.Close();
        }

        public void RollbackTransaction()
        {
            objCommand.Transaction.Rollback();
            objConnection.Close();
        }

        public int ExecuteNonQuery(string query)
        {
            return ExecuteNonQuery(query, CommandType.Text, ConnectionState.CloseOnExit);
        }

        public int ExecuteNonQuery(string query, CommandType commandtype)
        {
            return ExecuteNonQuery(query, commandtype, ConnectionState.CloseOnExit);
        }

        public int ExecuteNonQuery(string query, ConnectionState connectionstate)
        {
            return ExecuteNonQuery(query, CommandType.Text, connectionstate);
        }

        public int ExecuteNonQuery(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            int i = -1;
            try
            {
                if (objConnection.State == System.Data.ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                i = objCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    objConnection.Close();
                }
            }
            return i;
        }

        public long ExecuteNonQuery_id(string query)
        {
            objCommand.CommandText = query;
            objCommand.CommandType = CommandType.Text;
            try
            {
                if (objConnection.State == System.Data.ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                objCommand.ExecuteNonQuery();
                objCommand.CommandText = "select @@identity";
                return long.Parse(objCommand.ExecuteScalar().ToString());

            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {

                objCommand.Parameters.Clear();
                objCommand.Dispose();
                if (objConnection.State == System.Data.ConnectionState.Open)
                {
                    objConnection.Close();
                }
            }
            return -1;
        }

        public object ExecuteScalar(string query)
        {
            return ExecuteScalar(query, CommandType.Text, ConnectionState.CloseOnExit);
        }

        public object ExecuteScalar(string query, CommandType commandtype)
        {
            return ExecuteScalar(query, commandtype, ConnectionState.CloseOnExit);
        }

        public object ExecuteScalar(string query, ConnectionState connectionstate)
        {
            return ExecuteScalar(query, CommandType.Text, connectionstate);
        }

        public object ExecuteScalar(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            object o = null;
            try
            {
                if (objConnection.State == System.Data.ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                o = objCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    objConnection.Close();
                }
            }
            return o;
        }

        public DbDataReader ExecuteReader(string query)
        {
            return ExecuteReader(query, CommandType.Text, ConnectionState.CloseOnExit);
        }

        public DbDataReader ExecuteReader(string query, CommandType commandtype)
        {
            return ExecuteReader(query, commandtype, ConnectionState.CloseOnExit);
        }

        public DbDataReader ExecuteReader(string query, ConnectionState connectionstate)
        {
            return ExecuteReader(query, CommandType.Text, connectionstate);
        }

        public DbDataReader ExecuteReader(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            DbDataReader reader = null;
            try
            {
                if (objConnection.State == System.Data.ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    reader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                {
                    reader = objCommand.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
            }
            return reader;
        }

        public DataSet ExecuteDataSet(string query)
        {
            return ExecuteDataSet(query, CommandType.Text, ConnectionState.CloseOnExit);
        }

        public DataSet ExecuteDataSet(string query, CommandType commandtype)
        {
            return ExecuteDataSet(query, commandtype, ConnectionState.CloseOnExit);
        }

        public DataSet ExecuteDataSet(string query, ConnectionState connectionstate)
        {
            return ExecuteDataSet(query, CommandType.Text, connectionstate);
        }

        public DataSet ExecuteDataSet(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            DbDataAdapter adapter = objFactory.CreateDataAdapter();
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            adapter.SelectCommand = objCommand;
            DataSet ds = new DataSet();
            try
            {
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    if (objConnection.State == System.Data.ConnectionState.Open)
                    {
                        objConnection.Close();
                    }
                }
            }
            return ds;
        }

        private void HandleExceptions(Exception ex)
        {
            if (LogErrors)
            {
                WriteToLog(ex.Message);
            }
            if (HandleErrors)
            {
                strLastError = ex.Message;
            }
            else
            {
                throw ex;
            }
        }

        private void WriteToLog(string msg)
        {
            StreamWriter writer = File.AppendText(LogFile);
            writer.WriteLine(DateTime.Now.ToString() + " - " + msg);
            writer.Close();
        }

        public void Dispose1()
        {
            objConnection.Close();
            objConnection.Dispose();
            objCommand.Dispose();
        }
        void System.IDisposable.Dispose()
        {
            Dispose1();
        }


        public DataTable ExecuteDataTable(string query)
        {
            return ExecuteDataTable(query, CommandType.Text, ConnectionState.CloseOnExit);
        }

        public DataTable ExecuteDataTable(string query, CommandType commandtype)
        {
            return ExecuteDataTable(query, commandtype, ConnectionState.CloseOnExit);
        }

        public DataTable ExecuteDataTable(string query, ConnectionState connectionstate)
        {
            return ExecuteDataTable(query, CommandType.Text, connectionstate);
        }

        public DataTable ExecuteDataTable(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            DbDataAdapter adapter = objFactory.CreateDataAdapter();
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            adapter.SelectCommand = objCommand;
            DataTable dt = new DataTable();
            try
            {
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    if (objConnection.State == System.Data.ConnectionState.Open)
                    {
                        objConnection.Close();
                    }
                }
            }
            return dt;
        }

        public string GetProName(string Tablename, string ProName, string idName, string idValue)
        {
            string result = "";
            AddParameter("idv", idValue);
            DbDataReader r = ExecuteReader("select " + ProName + " from " + Tablename + " where " + idName + "=@idv");
            if (r.Read())
            {

                if (object.ReferenceEquals(r[ProName], DBNull.Value))
                {
                    result = "";
                }
                else
                {
                    result = r[ProName].ToString();
                }

            }
            else
            {
                result = "";
            }
            r.Close();

            return result;
        }

        public string[] getPageData(string pageid, string cul)
        {
            string[] result = new string[2];
            AddParameter("@pageid", pageid);
            DataTable dt = ExecuteDataTable("GetPageData", CommandType.StoredProcedure);
            if (dt.Rows.Count != 0)
            {
                if (cul.Contains("Arabic"))
                {
                    result[0] = dt.Rows[0]["title_ar"].ToString();
                    result[1] = dt.Rows[0]["txt_ar"].ToString();
                }
                else
                {
                    result[0] = dt.Rows[0]["title"].ToString();
                    result[1] = dt.Rows[0]["txt"].ToString();
                }
            }

            return result;

        }
    }
}