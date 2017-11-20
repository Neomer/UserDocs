using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserDocs.Models
{
    public class DocumentListViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string PrintableSize { get; set; }
        public string Filename { get; set; }
}
}