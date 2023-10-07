using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn.ViewModels
{
    public class FAQViewModel : ViewModelBase
    {

        public class FaqItem
        {
            public string Question { get; set; }
            public string Answer { get; set; }
        }

    }
}
