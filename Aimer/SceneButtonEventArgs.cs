using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aimer
{
    public class SceneButtonEventArgs : EventArgs
    {
        public string SceneId { get; set; }

        public SceneButtonEventArgs(string sceneId)
        {
            SceneId = sceneId;
        }
        
    }

    
}
