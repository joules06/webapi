using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    public class Utils
    {
        public static void ExecuteSPWithNoDataReturn(string procedureName, List<SqlParameter> parameters, SqlConnection dataBaseConnection)
        {
            SqlCommand command = new SqlCommand(procedureName, dataBaseConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            try
            {
                dataBaseConnection.Open();
                command.ExecuteNonQuery();
            }
            catch {}
            finally
            {
                dataBaseConnection.Close();
            }
        }

        public static void ExecuteSPWithNoDataReturn(string procedureName, List<SqlParameter> parameters, SqlConnection dataBaseConnection, ref bool flag)
        {
            flag = true;
            SqlCommand command = new SqlCommand(procedureName, dataBaseConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            try
            {
                dataBaseConnection.Open();
                command.ExecuteNonQuery();
            }
            catch { flag = false; }
            finally
            {
                dataBaseConnection.Close();
            }
        }

        public static DataTable ExecuteSPWithDataReturn(string procedureName, List<SqlParameter> parameters, SqlConnection dataBaseConnection)
        {
            DataTable dataTable = new DataTable();
            SqlCommand command = new SqlCommand(procedureName, dataBaseConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            SqlDataAdapter adapter = new SqlDataAdapter(command);

            try
            {
                dataBaseConnection.Open();
                adapter.Fill(dataTable);

            }
            catch{}
            finally
            {
                dataBaseConnection.Close();
            }

            return dataTable;
        }

        public static void ExecuteSPWithNoDataReturn(string procedureName, List<SqlParameter> parameters, SqlConnection dataBaseConnection, string paramOutPutName, ref int newProductID)
        {
            SqlCommand command = new SqlCommand(procedureName, dataBaseConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            SqlParameter outPutParameter = new SqlParameter
            {
                ParameterName = paramOutPutName,
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output,
            };

            command.Parameters.Add(outPutParameter);

            try
            {
                dataBaseConnection.Open();
                command.ExecuteNonQuery();
                int.TryParse(outPutParameter.Value.ToString(), out newProductID);
            }
            catch { }
            finally
            {
                dataBaseConnection.Close();
            }
        }

        public static ErrorCodeClass GetErrorMessage(int code)
        {
            ErrorCodeClass responseCode = new ErrorCodeClass
            {
                code = code
            };

            switch (code)
            {
                case 401:
                    responseCode.description = "User not found";
                    break;

                case 402:
                    responseCode.description = "Product not found";
                    break;

                case 403:
                    responseCode.description = "Unathorized";
                    break;

                case 404:
                    responseCode.description = "Not found";
                    break;

                case 500:
                    responseCode.description = "Unable to serialize posted data";
                    break;

                case 501:
                    responseCode.description = "Database error";
                    break;

                case 502:
                    responseCode.description = "Avialable product is not enough";
                    break;
            }

            return responseCode;
        }
    }
}
