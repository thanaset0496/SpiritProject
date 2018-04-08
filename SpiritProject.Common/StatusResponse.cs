using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class StatusResponse
    {
        public int status
        {
            get;
            set;
        }
        public string error_code
        {
            get;
            set;
        }
        public string description
        {
            get;
            set;
        }

        public decimal TotalScore
        {
            get;
            set;
        }

       
    }
}
