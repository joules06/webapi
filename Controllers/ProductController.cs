using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.InternalControllers;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public ListOfProductResponse Get([FromQuery] SortingTypes sortingBy = SortingTypes.ByName, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 0, [FromQuery] string wordToSearch = "-")
        {
            ProductsProcess controller = new ProductsProcess();

            return controller.GetProducts(sortingBy, wordToSearch, 0, pageIndex, pageSize);
        }

        [HttpGet("{id}")]
        public ListOfProductResponse Get(int id)
        {
            ProductsProcess controller = new ProductsProcess();

            return controller.GetProducts(SortingTypes.ByName, "-", id);
        }

        [HttpPost]
        public NewProductResponse Post([FromHeader] string userName, [FromHeader] string password, [FromBody] ProductClassBase newProduct)
        {
            ProductsProcess controller = new ProductsProcess();

            return controller.CreateProduct(userName, password, newProduct);
        }

        [HttpPut]
        public UpdateProductResponse Put([FromHeader] string userName, [FromHeader] string password, [FromBody] ProductClassForUpdate product)
        {
            ProductsProcess controller = new ProductsProcess();

            return controller.UpdateProduct(userName, password, product);
        }

        [HttpDelete("{id}")]
        public DeleteProductResponse Delete([FromHeader] string userName, [FromHeader] string password, int id)
        {
            ProductsProcess controller = new ProductsProcess();

            return controller.DeleteProduct(userName, password, id);
        }
    }
}