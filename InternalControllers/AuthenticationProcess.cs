using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.Models;

namespace WebAPI.InternalControllers
{
    public class AuthenticationProcess
    {
        public bool Authenticate(string user, string password, UserLevel userLevel, ref List<ResponseErrorClass> errorList)
        {
            bool isValid = false;
            
            SqlConnection connection = new SqlConnection
            {
                ConnectionString = GlobalVariables.ConnectionString
            };

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                        ParameterName = "@Name",
                        DbType = DbType.String,
                        Value = user ?? ""
                    },
                new SqlParameter
                {
                        ParameterName = "@Password",
                        DbType = DbType.String,
                        Value = password ?? ""
                    },
                new SqlParameter
                {
                        ParameterName = "@UserLevel",
                        DbType = DbType.Int32,
                        Value = (int)userLevel
                    },
            };

            int exists = -1;

            Utils.ExecuteSPWithNoDataReturn("dbo.CHECK_USER", parameters, connection, "@Exists", ref exists);

            isValid = exists == 1 ? true : false;

            if (!isValid)
            {
                ErrorCodeClass code = Utils.GetErrorMessage(403);
                errorList.Add(new ResponseErrorClass
                {
                    Message = code.description,
                    Code = code.code
                });
            }

            return isValid;
        }
    }
}
