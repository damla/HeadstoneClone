using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Responses
{
    public class FavoriteProductServiceResponse<T> : ServiceResponse<T>
    {
        public int FavId { get; set; }
    }
}