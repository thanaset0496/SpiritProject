using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class MemberModel
    {
        public string MemberId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string MemberNameTH { get; set; }
        public string MemberLastNameTH { get; set; }
        public string MemberNameEN { get; set; }
        public string MemberLastNameEN { get; set; }
        public DateTime? MemberDOB { get; set; }
        public string Role { get; set; }
        public int Status { get; set; }
        public int FailLogin { get; set; }
        public string FullName { get; set; }

        public string MemberName { get; set; }
        public string MemberLastName { get; set; }
        public string Email { get; set; }


        public string LastAccess { get; set; }
        public int Minunte { get; set; }
        public string StartDate { get; set; }
        public int Second { get; set; }
        public int TimeSet { get; set; }
        public int TotalItem { get; set; }
        public string Message { get; set; }

    }
}
