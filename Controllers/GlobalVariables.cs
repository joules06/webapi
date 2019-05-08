using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    public class GlobalVariables
    {
        public static string ConnectionString = @"Data Source=.;Initial Catalog=ProductsDemo;User ID=sa; Password=pa$$w0rd";
    }

    public enum SortingTypes
    {
        ByName = 1, ByLikes = 2
    }

    public enum UpdateType
    {
        All = 1, Price = 2, Likes = 3
    }

    public enum UserLevel
    {
        Administrator = 1, Basic = 2
    }

    public class ErrorCodeClass
    {
        public string description { get; set; }
        public int code { get; set; }
    }
}
