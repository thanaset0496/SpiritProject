using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }
        public string DataTH { get; set; }
        public string DataEN { get; set; }
        public int TestNo { get; set; }
        public int FactorId{ get; set; }
        public int SubFactorId { get; set; }
        public int isFinal { get; set; }
        public string UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public string QuestionStatus { get; set; }


        public string FactorNameTH { get; set; }
        public string FactorNameEN { get; set; }

        public string SubFactorNameTH { get; set; }
        public string SubFactorNameEN { get; set; }

        public int TotalItem { get; set; }


        public List<ChoiceModel> Choice { get; set; }
    }
}
