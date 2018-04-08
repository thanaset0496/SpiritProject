using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
  public  class ExamModel
    {
        public int ExamId { get; set; }
        public string PrefixName { get; set; }
        public string FullName { get; set; }
        public int TotalQuestion { get; set; }
        public decimal TotalScore { get; set; }
        public int TotalTime { get; set; }
        public string Exam { get; set; }
        public DateTime AddDate { get; set; }
        public string ExamStatus { get; set; }
        public int Remind1 { get; set; }
        public int Remind2 { get; set; }
    }
}
