using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ResponseErrorClass
    {
        [JsonProperty(PropertyName = "Code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }
    }
}
