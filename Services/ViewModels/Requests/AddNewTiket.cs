using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.ViewModels.Requests
{
   public class AddNewTiket
    {
        [Required]
        [StringLength(100)]

        public string Title { get; set; }
        [Required]
        public short Level { get; set; }
        [Required]
        public short Department { get; set; }

    }
}
