using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace pz3.Models
{
    public class GeographicModel
    {
        public List<SubstationEntity> Substations { get; set; }
        public List<NodeEntity> Nodes { get; set; }
        public List<SwitchEntity> Switches { get; set; }
        public List<LineEntity> Lines { get; set; }


        private XDocument xdoc;
        private LonLatConverter lonLat;
        public double mapLeft { get; set; }
        public double mapRight { get; set; }
        public double mapTop { get; set; }
        public double mapBottom { get; set; }
        public double intervalX { get; set; }
        public double intervalY { get; set; }
        public double mapEdgeSize { get; set; }

        public GeographicModel()
        {
            this.mapLeft = 19.793909;
            this.mapRight = 19.894459;
            this.mapTop = 45.277031;
            this.mapBottom = 45.2325;
            intervalX = mapRight - mapLeft;
            intervalY = mapTop - mapBottom;
            mapEdgeSize = 2;
            xdoc = XDocument.Load("Geographic.xml");
            lonLat = new LonLatConverter();
            Nodes = LoadNodes().Where(x => IsValidPowerEntity(x.X, x.Y) == true).ToList();
            Substations = LoadSubstations().Where(x => IsValidPowerEntity(x.X, x.Y) == true).ToList();
            Switches = LoadSwitches().Where(x => IsValidPowerEntity(x.X, x.Y) == true).ToList();
            Lines = LoadLines();

        }
        public bool IsValidPowerEntity(double x, double y)
        {
            return (y >= mapBottom && y <= mapTop) && (x >= mapLeft && x <= mapRight);
        }

        public List<LineEntity> LoadLines()
        {
            var lines = xdoc.Descendants("LineEntity")
                      .Select(line => new LineEntity
                      {
                          Id = (UInt64)line.Element("Id"),
                          Name = (string)line.Element("Name"),
                          ConductorMaterial = (string)line.Element("ConductorMaterial"),
                          IsUnderground = (bool)line.Element("IsUnderground"),
                          R = (float)line.Element("R"),
                          FirstEnd = (UInt64)line.Element("FirstEnd"),
                          SecondEnd = (UInt64)line.Element("SecondEnd"),
                          LineType = (string)line.Element("LineType"),
                          ThermalConstantHeat = (UInt64)line.Element("ThermalConstantHeat"),
                          Vertices = line.Element("Vertices").Descendants("Point").Select(p => new Point
                          {
                              X = (double)p.Element("X"),
                              Y = (double)p.Element("Y"),
                          }).ToList()
                      }).ToList();
            foreach (var line in lines)
            {
                double lat, lon;
                foreach (var point in line.Vertices)
                {

                    lonLat.ToLatLon(point.X, point.Y, 34, out lat, out lon);
                    point.X = lon;
                    point.Y = lat;
                }
            }



            List<LineEntity> allLines = new List<LineEntity>();
            foreach (var item in lines)
            {
                bool firstEnd = false;
                bool secondEnd = false;
                foreach (SubstationEntity sub in Substations)
                {
                    if (sub.Id == item.FirstEnd)
                    {
                        firstEnd = true;
                    }
                    if (sub.Id == item.SecondEnd)
                    {
                        secondEnd = true;
                    }
                }

                foreach (NodeEntity node in Nodes)
                {
                    if (node.Id == item.FirstEnd)
                    {
                        firstEnd = true;
                    }
                    if (node.Id == item.SecondEnd)
                    {
                        secondEnd = true;
                    }
                }

                foreach (SwitchEntity sw in Switches)
                {
                    if (sw.Id == item.FirstEnd)
                    {
                        firstEnd = true;
                    }
                    if (sw.Id == item.SecondEnd)
                    {
                        secondEnd = true;
                    }
                }

                if (firstEnd == true && secondEnd == true)
                {
                    if (!allLines.Contains(item))
                        allLines.Add(item);
                }
            }




            return allLines;
        }

        public List<NodeEntity> LoadNodes()
        {
            var nodes = xdoc
                     .Descendants("NodeEntity")
                     .Select(node => new NodeEntity
                     {
                         Id = (UInt64)node.Element("Id"),
                         Name = (string)node.Element("Name"),
                         X = (double)node.Element("X"),
                         Y = (double)node.Element("Y"),
                     }).ToList();
            foreach (var item in nodes)
            {
                lonLat.ToLatLon(item.X, item.Y, 34, out double lat, out double lon);
                item.X = lon;
                item.Y = lat;
            }
            return nodes;
        }

        public List<SubstationEntity> LoadSubstations()
        {
            var substations = xdoc
                    .Descendants("SubstationEntity")
                    .Select(sub => new SubstationEntity
                    {
                        Id = (UInt64)sub.Element("Id"),
                        Name = (string)sub.Element("Name"),
                        X = (double)sub.Element("X"),
                        Y = (double)sub.Element("Y"),
                    }).ToList();
            foreach (var item in substations)
            {
                lonLat.ToLatLon(item.X, item.Y, 34, out double lat, out double lon);
                item.X = lon;
                item.Y = lat;
            }
            return substations;
        }

        public List<SwitchEntity> LoadSwitches()
        {
            var switches = xdoc
                     .Descendants("SwitchEntity")
                     .Select(sw => new SwitchEntity
                     {
                         Id = (UInt64)sw.Element("Id"),
                         Name = (string)sw.Element("Name"),
                         Status = (string)sw.Element("Status"),
                         X = (double)sw.Element("X"),
                         Y = (double)sw.Element("Y"),
                     }).ToList();
            foreach (var item in switches)
            {
                lonLat.ToLatLon(item.X, item.Y, 34, out double lat, out double lon);
                item.X = lon;
                item.Y = lat;
            }
            return switches;
        }

    }
}
