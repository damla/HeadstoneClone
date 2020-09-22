using Headstone.Models;
using Headstone.Framework.Data.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.DataAccess
{
    public class CommentPropertyDAO : EFDataChannel<CommentProperty, HeadstoneDbContext>
    {
    }
}
