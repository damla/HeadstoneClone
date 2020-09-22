using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Favorites
{
    public class FavoriteProductCreated : BaseEvent
    {
        public int UserId { get; set; }

        public int ProductId { get; set; }
    }
}
