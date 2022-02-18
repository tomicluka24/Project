using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace pz3.Models
{
    public class EntityModel
    {
        public EntityType Type { get; set; }
        public GeometryModel3D Model { get; set; }
        public EntityModel(EntityType type, GeometryModel3D model)
        {
            this.Type = type;
            this.Model = model;
        }
    }
}
