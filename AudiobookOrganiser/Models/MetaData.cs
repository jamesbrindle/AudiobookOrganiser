using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiobookOrganiser.Models
{
    internal class MetaData
    {
        public string Author { get; set; }        
        public string Narrator { get; set; }
        public string Title { get; set; }
        public string Series { get; set; }
        public string SeriesPart { get; set; }
        public string Year { get; set; }
    }
}
