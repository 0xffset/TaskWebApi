using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taks.Core.Entities.ViewModels
{
    public class AuthResultViewModel
    {
        public string? AccessToken { get; set; }
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
    }
}
