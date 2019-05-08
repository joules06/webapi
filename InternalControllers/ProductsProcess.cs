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
    public class ProductsProcess
    {
        public ListOfProductResponse GetProducts(SortingTypes sortingBy, string wordToSerach, int productID = 0, int pagingIndex = 0, int pagingSize = 0)
        {
            ListOfProductResponse response = new ListOfProductResponse();
            List<ResponseErrorClass> errorsList = new List<ResponseErrorClass>();
            errorsList.Clear();

            SqlConnection connection = new SqlConnection
            {
                ConnectionString = GlobalVariables.ConnectionString
            };

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                        ParameterName = "@ById",
                        DbType = DbType.Int32,
                        Value = productID
                },
                new SqlParameter
                {
                        ParameterName = "@PageIndex",
                        DbType = DbType.Int32,
                        Value = pagingIndex
                },
                new SqlParameter
                {
                        ParameterName = "@PageSize",
                        DbType = DbType.Int32,
                        Value = pagingSize
                },
                new SqlParameter
                {
                        ParameterName = "@WordToSerach",
                        DbType = DbType.String,
                        Value = wordToSerach
                },
                new SqlParameter
                {
                        ParameterName = "@PagingEnabled",
                        DbType = DbType.Int32,
                        Value = (pagingIndex >= 0 && pagingSize > 0) ? 1 : 0
                },
            };

            DataTable table = productID == 0 ? Utils.ExecuteSPWithDataReturn("dbo.GET_PRODUCTS", parameters, connection) : Utils.ExecuteSPWithDataReturn("dbo.GET_PRODUCTS", parameters, connection);
            List<ProductClass> list = new List<ProductClass>();

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    int.TryParse(row["product_id"].ToString(), out int id);
                    int.TryParse(row["quantity"].ToString(), out int quantity);
                    double.TryParse(row["price"].ToString(), out double price);
                    int.TryParse(row["likes"].ToString(), out int likes);

                    list.Add(new ProductClass
                    {
                        Id = id,
                        Name = row["name"].ToString(),
                        Quantity = quantity,
                        Price = price,
                        Likes = likes,
                    });
                }

                if (sortingBy == SortingTypes.ByName)
                {
                    list = list.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    list = list.OrderBy(x => x.Likes).ToList();
                }

                response.Products = list;
            }
            else
            {
                ErrorCodeClass code = Utils.GetErrorMessage(404);
                errorsList = new List<ResponseErrorClass>
                {
                    new ResponseErrorClass
                    {
                        Code = code.code,
                        Message = code.description,
                    }
                };
            }

            response.Errors = errorsList;

            return response;
        }

        public NewProductResponse CreateProduct(string userName, string password, ProductClassBase newProduct)
        {
            NewProductResponse response = new NewProductResponse();
            List<ResponseErrorClass> errorsList = new List<ResponseErrorClass>();
            errorsList.Clear();

            AuthenticationProcess authenticationProcess = new AuthenticationProcess();
            bool isValid = authenticationProcess.Authenticate(userName, password, UserLevel.Administrator, ref errorsList);

            if (isValid)
            {
                if (newProduct != null)
                {
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
                                Value = newProduct.Name
                         },
                        new SqlParameter
                        {
                                ParameterName = "@Price",
                                DbType = DbType.Decimal,
                                Value = newProduct.Price
                         },
                        new SqlParameter
                        {
                                ParameterName = "@Quantity",
                                DbType = DbType.Int32,
                                Value = newProduct.Quantity
                         },
                    };

                    int newProductID = -1;

                    Utils.ExecuteSPWithNoDataReturn("dbo.SET_PRODUCTS", parameters, connection, "@Id", ref newProductID);
                    response.ID = newProductID;
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

        public UpdateProductResponse UpdateProduct(string userName, string password, ProductClassForUpdate product)
        {
            UpdateProductResponse response = new UpdateProductResponse();
            bool updated = false;
            List<ResponseErrorClass> errorsList = new List<ResponseErrorClass>();
            errorsList.Clear();

            UserLevel userLevel = UserLevel.Administrator;
            if (product != null)
            {
                userLevel = product.Type == UpdateType.Likes ? UserLevel.Basic : UserLevel.Administrator;
            }
            AuthenticationProcess authenticationProcess = new AuthenticationProcess();
            bool isValid = authenticationProcess.Authenticate(userName, password, userLevel, ref errorsList);
            
            if (isValid)
            {
                if (product != null)
                {
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
                            Value = String.IsNullOrEmpty(product.Name) ? "-" : product.Name
                         },
                        new SqlParameter
                        {
                            ParameterName = "@Price",
                            DbType = DbType.Decimal,
                            Value = product.Price
                         },
                        new SqlParameter
                        {
                            ParameterName = "@Quantity",
                            DbType = DbType.Int32,
                            Value = product.Quantity
                         },
                        new SqlParameter
                        {
                            ParameterName = "@Id",
                            DbType = DbType.Int32,
                            Value = product.Id
                         },
                        new SqlParameter
                        {
                            ParameterName = "@UpdateType",
                            DbType = DbType.Int32,
                            Value = (int)product.Type
                         },
                    };
                    Utils.ExecuteSPWithNoDataReturn("dbo.UPDATE_PRODUCT", parameters, connection, ref updated);

                    if (!updated)
                    {
                        ErrorCodeClass code = Utils.GetErrorMessage(501);
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
            response.Updated = updated;

            return response;
        }

        public DeleteProductResponse DeleteProduct(string userName, string password, int productID)
        {
            bool deleted = false;
            DeleteProductResponse response = new DeleteProductResponse();
            List<ResponseErrorClass> errorsList = new List<ResponseErrorClass>();
            errorsList.Clear();

            AuthenticationProcess authenticationProcess = new AuthenticationProcess();
            bool isValid = authenticationProcess.Authenticate(userName, password, UserLevel.Administrator, ref errorsList);

            if (isValid)
            {
                SqlConnection connection = new SqlConnection
                {
                    ConnectionString = GlobalVariables.ConnectionString
                };

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter
                    {
                            ParameterName = "@Id",
                            DbType = DbType.Int32,
                            Value = productID
                    },
                };

                Utils.ExecuteSPWithNoDataReturn("dbo.DELETE_PRODUCT", parameters, connection, ref deleted);

                if (!deleted)
                {
                    ErrorCodeClass code = Utils.GetErrorMessage(501);
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
            response.Deleted = deleted;

            return response;
        }
    }
}
