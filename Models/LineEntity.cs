using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pz3.Models
{
    public class LineEntity
    {
        private UInt64 id;
        private string name;
        private bool isUnderground;
        private float r;
        private string conductorMaterial;
        private string lineType;
        private UInt64 thermalConstantHeat;
        private UInt64 firstEnd;
        private UInt64 secondEnd;
        private List<Point> vertices;

        public LineEntity()
        {

        }

        public UInt64 Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public bool IsUnderground
        {
            get
            {
                return isUnderground;
            }

            set
            {
                isUnderground = value;
            }
        }

        public float R
        {
            get
            {
                return r;
            }

            set
            {
                r = value;
            }
        }

        public string ConductorMaterial
        {
            get
            {
                return conductorMaterial;
            }

            set
            {
                conductorMaterial = value;
            }
        }

        public string LineType
        {
            get
            {
                return lineType;
            }

            set
            {
                lineType = value;
            }
        }

        public UInt64 ThermalConstantHeat
        {
            get
            {
                return thermalConstantHeat;
            }

            set
            {
                thermalConstantHeat = value;
            }
        }

        public UInt64 FirstEnd
        {
            get
            {
                return firstEnd;
            }

            set
            {
                firstEnd = value;
            }
        }

        public UInt64 SecondEnd
        {
            get
            {
                return secondEnd;
            }

            set
            {
                secondEnd = value;
            }
        }

        public List<Point> Vertices
        {
            get
            {
                return vertices;
            }

            set
            {
                vertices = value;
            }
        }
    }
}
