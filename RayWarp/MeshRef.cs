using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.RayWarp
{
    public class MeshRef
    {
        public Raylib_cs.Mesh Mesh;
        public MeshRef() {}
        public MeshRef(Raylib_cs.Mesh mesh)
        {
            Mesh = mesh;
        }
    }
}
