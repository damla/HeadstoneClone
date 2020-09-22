using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class BasketItemRequest : BaseRequest
    {
        public List<int> BasketItemIDs { get; set; } = new List<int>();

        public List<int> BasketIDs { get; set; } = new List<int>();

        public List<int> ProductIDs { get; set; } = new List<int>();
        // buraya bakilacak
    }
}
