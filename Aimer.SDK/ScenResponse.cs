using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aimer.SDK
{
    public class ScenResponse
    {
        public string response { get; set; }
        public Dictionary<string,Scene> scenslist { get; set; }
    }
}
