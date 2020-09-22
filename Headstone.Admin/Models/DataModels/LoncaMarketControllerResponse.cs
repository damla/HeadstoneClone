using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.DataModels
{
    public class headstoneControllerResponse<T>
    {
        public int ReturnCode { get; set; }

        public List<T> Result { get; set; } = new List<T>();
    }

    public class headstoneControllerResponse
    {
        public int ReturnCode { get; set; }
    }
}