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
        public string Role { get; set; }
        public string Action { get; set; }
        public int MaxFail { get; set; }
        public int MinPass { get; set; }
        public string Power_State { get; set; }
        public int Reports { get; set; }
        public int Passes { get; set; }
        public int Fails { get; set; }

    }
}
