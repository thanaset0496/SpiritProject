using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class AnswerSheetModel
    {
        public string MemberId { get; set; }
        public int TestNo { get; set; }
        public int ExamId { get; set; }
        public int QuestionOrder { get; set; }
        public int QuestionId { get; set; }
        public int? ChoiceId { get; set; }
        public string ChoiceType { get; set; }
        public decimal? Point { get; set; }
        public decimal? TimeUsed { get; set; }
        public string QuestionName { get; set; }
        public List<ChoiceModel> Choice { get; set; }

      //  public QuestionModel Question { get; set; }
        public int isFinal { get; set; }
        public int FakeChoiceId { get; set; }
    }
}
