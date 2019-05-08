using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Models
{
    public class ProductClassBase
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }
    }
    public class ProductClass : ProductClassBase
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public int Likes { get; set; }
    }

    public class ProductClassForUpdate: ProductClass
    {
        [JsonProperty(PropertyName = "type")]
        public UpdateType Type { get; set; }
    }

    public class ListOfProductResponse
    {
        [JsonProperty(PropertyName = "products")]
        public List<ProductClass> Products { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<ResponseErrorClass> Errors { get; set; }
    }

    public class NewProductResponse
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<ResponseErrorClass> Errors { get; set; }
    }

    public class UpdateProductResponse
    {
        [JsonProperty(PropertyName = "updated")]
        public bool Updated { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<ResponseErrorClass> Errors { get; set; }
    }

    public class DeleteProductResponse
    {
        [JsonProperty(PropertyName = "deleted")]
        public bool Deleted { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<ResponseErrorClass> Errors { get; set; }
    }

    public class PurchaseProductRequest
    {
        [JsonProperty(PropertyName = "product_id")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

    }
    public class PurchaseProductResponse
    {
        [JsonProperty(PropertyName = "transaction_id")]
        public int TransactionID { get; set; }

        [JsonProperty(PropertyName = "item_purchased")]
        public bool ItemPurchased { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<ResponseErrorClass> Errors { get; set; }
    }
}
