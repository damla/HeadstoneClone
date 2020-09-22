using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Headstone.MetaData.API.Models.Live;
using Headstone.MetaData.Common.Models;

namespace Headstone.AI.Models.ViewModels
{
    public class ContentViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public int ContentId { get; internal set; }
        public string LongDescription { get; internal set; }
        public string ShortDescription { get; internal set; }
    }
}