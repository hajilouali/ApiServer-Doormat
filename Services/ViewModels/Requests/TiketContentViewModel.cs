using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.ViewModels.Requests
{
   public class TiketContentViewModel
    {
        [StringLength(1000)]
        public string Text { get; set; }

        public IFormFile File { get; set; }
    }
}
