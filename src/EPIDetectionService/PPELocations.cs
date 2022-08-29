using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPIDetection
{
    public class PPELocations
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public float Condifence { get; set; }


        public PPELocations()
        {

        }

        public PPELocations(int x, int y, int width, int height, float condifence, string name)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = name;
            Condifence = condifence;
        }
    }
}
