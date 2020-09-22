using Headstone.Framework.Data.Services;
using Headstone.Models;
using Headstone.Service.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Base
{
    public class BasketServiceBase : EFServiceBase<Basket, BasketDAO, HeadstoneDbContext>
    {

    }
}