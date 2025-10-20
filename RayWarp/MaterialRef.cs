using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.RayWarp
{
    public class MaterialRef
    {
        public Raylib_cs.Material Material;
        public MaterialRef(Raylib_cs.Material material)
        {
            Material = material;
        }
    }
}
