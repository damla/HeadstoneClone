using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class BasketRequest : BaseRequest
    {
        public List<int> BasketIds { get; set; } = new List<int>();

        public List<int> UserIds { get; set; } = new List<int>();

        public List<BasketItem> BasketItemIds { get; set; } = new List<BasketItem>();
    }
}