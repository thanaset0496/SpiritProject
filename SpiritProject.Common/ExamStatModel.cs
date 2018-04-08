using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class ExamStatModel
    {
        public string MemberId { get; set; }
        public int TestNo { get; set; }
        public int ExamId { get; set; }
        public decimal? ScoreSet { get; set; }
        public decimal? ScoreGet { get; set; }
        public int? TimeUsed { get; set; }
        public int? TimeSet { get; set; }
        public DateTime? StartDate { get; set; }
        public string DisplayStartDate { get { return StartDate == null ? "00/00/0000" : DateTimeFormat.DateFormatToString(StartDate); } }
        public DateTime? FinishedDate { get; set; }
        public string DisplayFinishedDate { get { return FinishedDate == null ? "00/00/0000" : DateTimeFormat.DateFormatToString(FinishedDate); } }
        public int? PageNo { get; set; }
        public string Status { get; set; }
        public string TestStatus { get; set; }

        public int Answered { get; set; }
        public int TotalQuestion { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalScore { get; set; }
        public string ExamDesc { get; set; }

        public int Times { get; set; }
        public string FullName { get; set; }
        public DateTime? LastAccess { get; set; }
        public string LastAccessDisplay { get { return LastAccess == null ? "00/00/0000 00:00:00" : DateTimeFormat.DateFormatToString(LastAccess); } }

        public string MemberName { get; set; }

        public int Minute { get; set; }
        public int Second { get; set; }

        public decimal MonthDifference { get; set; }

        public int ExamAccessTimes { get; set; }
        public int Remind1 { get; set; }
        public int Remind2 { get; set; }

        public string FactorShortName1 { get; set; }
        public decimal ScoreFac1 { get; set; }
        public decimal Norm1 { get; set; }
        public string FactorShortName2 { get; set; }
        public decimal ScoreFac2 { get; set; }
        public decimal Norm2 { get; set; }
        public string FactorShortName3 { get; set; }
        public decimal ScoreFac3 { get; set; }
        public decimal Norm3 { get; set; }
        public string FactorShortName4 { get; set; }
        public decimal ScoreFac4 { get; set; }
        public decimal Norm4 { get; set; }
        public string FactorShortName5 { get; set; }
        public decimal ScoreFac5 { get; set; }
        public decimal Norm5 { get; set; }
        public string FactorShortName6 { get; set; }
        public decimal ScoreFac6 { get; set; }
        public decimal Norm6 { get; set; }


    }
}
