using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared.ViewModels
{
    public class PowerHistory
    {
        public int LookbackDays { get; set; }
        public Dictionary<int,int> PowerPoints { get; set; }
    }
}
