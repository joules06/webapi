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
    [Route("api/buy")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        [HttpPost]
        public PurchaseProductResponse Buy([FromHeader] string userName, [FromHeader] string password, [FromBody] PurchaseProductRequest productToBuy)
        {
            BuyProcess controller = new BuyProcess();

            return controller.BuyProduct(userName, password, productToBuy);
        }
    }
}