using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class SummaryScoreModel
    {
        public int FactorId { get; set; }
        public int SubFactorId { get; set; }
        public string FactorName { get; set; }
        public string FactorShortName { get; set; }
        public string SubFactorName { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public decimal TotalScore { get; set; }
        public string TimeUsed { get; set; }
        public string TotalTimeUsed { get; set; }
        public int DoneQuestion { get; set; }
        public int TotalQuestion { get; set; }
        public string ExamFullName { get; set; }
        public string FinishedDate { get; set; }

        public int Level { get; set; }
        public string Mean { get; set; }
        public Decimal ExamNorm { get; set; }
    }
}
