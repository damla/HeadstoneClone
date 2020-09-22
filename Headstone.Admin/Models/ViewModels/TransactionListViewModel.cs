using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class TransactionListViewModel : BaseViewModel
    {
        public List<TransactionViewModel> Transactions { get; set; } = new List<TransactionViewModel>();

    }
}