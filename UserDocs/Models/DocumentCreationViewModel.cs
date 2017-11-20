using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace UserDocs.Models
{
    public class DocumentCreationViewModel
    {
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Файл")]
        public HttpPostedFileBase File { get; set; }

    }
}