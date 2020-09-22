using Headstone.Framework.Data.Channels;
using Headstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.DataAccess
{
    public class BasketItemDAO : EFDataChannel<BasketItem, HeadstoneDbContext>
    {
    }
}
