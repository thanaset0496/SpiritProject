using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class ChoiceModel
    {
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public string ChoiceName { get; set; }
        public string ChoiceType { get; set; }
        public int Point { get; set; }
        public string Status { get; set; }

        public string ChoiceNameTH { get; set; }
        public string ChoiceNameEN { get; set; }


        //     public int Ansewered { get; set; }
    }
}
