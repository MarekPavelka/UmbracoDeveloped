using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoDeveloped.Core.ViewModels
{
    public class ContactFormViewModel
    {
        [Required]
        [MaxLength(80, ErrorMessage="Please limit to 80 chars")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage="Please limit your comments to 500 chars")]
        public string Comment { get; set; }

        public string Subject { get; set; }
    }
}
