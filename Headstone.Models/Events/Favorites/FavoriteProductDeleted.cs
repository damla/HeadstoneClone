using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Favorites
{
    public class FavoriteProductDeleted : BaseEvent
    {
        public int FavoriteId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }
    }
}
