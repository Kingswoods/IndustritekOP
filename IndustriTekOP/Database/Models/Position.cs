using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Database.Models
{
    public class Position
    {
        public Position(double x, double y, double rotation)
        {
            this.PosX = x;
            this.PosY = y;
            this.Rotation = rotation;
        }

        public Position()
        {

        }

        public double PosX { get; set; }
        public double PosY { get; set; }
        public double Rotation { get; set; }
    }
}
