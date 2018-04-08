using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class DataTableAjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<column> columns { get; set; }
        public search search { get; set; }
        public List<order> order { get; set; }
    }

    public class column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public search search { get; set; }
    }

    public class search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class order
    {
        public string column { get; set; }
        public string dir { get; set; }
    }
}
