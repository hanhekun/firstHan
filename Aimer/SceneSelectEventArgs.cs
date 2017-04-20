using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aimer
{
    public class SceneSelectEventArgs:EventArgs
    {
        public int Index { get; set; }

        public SceneSelectEventArgs(int index)
        {
            Index = index;
        }
    }
}
