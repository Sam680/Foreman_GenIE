using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foreman_GenIE
{
    public class Machine
    {
        public string Name { get; set; }
        public string Environment { get; set; }
        public string Catagory { get; set; }
        public string Role { get; set; }
        public string Action { get; set; }
        public bool Passed { get; set; }
        public int MaxFail { get; set; }
        public string Power_State { get; set; }
        public int Reports { get; set; }
        public int Recent_Reports { get; set; }
        public int Fails { get; set; }
        public string Failure { get; set; }
        public bool Skipped { get; set; }
        public bool First_Run { get; set; }
        public bool Deploy { get; set; }

    }
}
