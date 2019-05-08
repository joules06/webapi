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
    public class BuyProcess
    {
        public PurchaseProductResponse BuyProduct(string userName, string password, PurchaseProductRequest productToBuy)
        {
            PurchaseProductResponse response = new PurchaseProductResponse();
            List<ResponseErrorClass> errorsList = new List<ResponseErrorClass>();
            errorsList.Clear();

            AuthenticationProcess authenticationProcess = new AuthenticationProcess();
            bool isValid = authenticationProcess.Authenticate(userName, password, UserLevel.Basic, ref errorsList);

            if (isValid)
            {
                if (productToBuy != null)
                {
                    SqlConnection connection = new SqlConnection
                    {
                        ConnectionString = GlobalVariables.ConnectionString
                    };

                    List<SqlParameter> parameters = new List<SqlParameter>
                    {
                        new SqlParameter
                        {
                                ParameterName = "@ProductId",
                                DbType = DbType.Int32,
                                Value = productToBuy.ProductId
                         },
                        new SqlParameter
                        {
                                ParameterName = "@UserdId",
                                DbType = DbType.String,
                                Value = userName
                         },
                        new SqlParameter
                        {
                                ParameterName = "@Quantity",
                                DbType = DbType.Int32,
                                Value = productToBuy.Quantity
                         },
                    };

                    int flag = -1;

                    Utils.ExecuteSPWithNoDataReturn("dbo.BUY_A_PRODUCT", parameters, connection, "@Flag", ref flag);
                    ErrorCodeClass code = Utils.GetErrorMessage(401);
                    switch (flag)
                    {
                        case -1:
                            code = Utils.GetErrorMessage(401);
                            errorsList = new List<ResponseErrorClass>
                            {
                                new ResponseErrorClass
                                {
                                    Code = code.code,
                                    Message = code.description,
                                }
                            };
                            break;
                        case -2:
                            code = Utils.GetErrorMessage(502);
                            errorsList = new List<ResponseErrorClass>
                            {
                                new ResponseErrorClass
                                {
                                    Code = code.code,
                                    Message = code.description,
                                }
                            };
                            break;
                        case -3:
                            code = Utils.GetErrorMessage(402);
                            errorsList = new List<ResponseErrorClass>
                            {
                                new ResponseErrorClass
                                {
                                    Code = code.code,
                                    Message = code.description,
                                }
                            };
                            break;
                    }

                    response.TransactionID = flag;
                    response.ItemPurchased = flag > 0 ? true : false;
                }
                else
                {
                    ErrorCodeClass code = Utils.GetErrorMessage(500);
                    errorsList = new List<ResponseErrorClass>
                    {
                        new ResponseErrorClass
                        {
                            Code = code.code,
                            Message = code.description,
                        }
                    };
                }

                
            }

            response.Errors = errorsList;
            return response;
        }
    }
}
