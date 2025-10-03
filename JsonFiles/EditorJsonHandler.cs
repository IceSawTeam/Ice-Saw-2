using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IceSaw2.Manager.Tricky.TrickyWorldManager;

namespace IceSaw2.JsonFiles
{
    [System.Serializable]
    public class EditorJsonHandler
    {
        //Archys Dummy file to figure out what would need to be saved and how to save it
        //TBD if used

        public int MajorVersion;
        public int MinorVersion;

        public Viewport LevelViewport;
        public TreeNode TreeNodes;

        public WindowMode ActiveWindow;
        public int SelectionNodeID; //Swapped out for list maybe?

        public struct Viewport
        {

        }
        

        public struct TreeNode
        {
            public string Name;
            public int NodeID;
            public int ObjectID; //-1 is no tricky object

            public List<TreeNode> treeNodes;
        }
    }
}
