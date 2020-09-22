using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Responses
{
    public class CommentServiceResponse<T> : ServiceResponse<T>
    {
        public int CommentId { get; set; }

        public int PropertyId { get; set; }

        public int TagId { get; set; }
    }
}
