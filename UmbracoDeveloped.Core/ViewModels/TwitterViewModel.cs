using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoDeveloped.Core.ViewModels
{
    public class TwitterViewModel
    {
        public string TwitterHandle { get; set; }
        public bool HasError { get; set; }
        public string Json { get; set; }
        public string Message { get; set; }
    }
}
