using IndustriTekOP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP
{
    class Navigator
    {
        public Navigator()
        {

        }

        public Position GetVector(double dX, double dY, double pX, double pY)
        {
            Position vector = new Position(dX - pX, dY - pY, 0);

            return vector;
        }

        public int GetSteps(double distance)
        {
            double stepLength = (Properties.Settings.Default.wheelDiameter * Math.PI) / 200;

            return Convert.ToInt32(distance / stepLength);
        }

        public double GetRotation(double dX, double dY, double cR)
        {
            double dotProduct = ((1 * dX) + (0 * dY));
            double rotationRadians = Math.Acos(dotProduct / GetVectorLength(dX, dY));
            double rotation = rotationRadians * (180 / Math.PI);

            if(dY < 0)
            {
                double extra = (180 - rotation);
                rotation = 180 + extra;
            }

            return rotation;
        }

        public double GetVectorLength(double dX, double dY)
        {
            return Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
        }

        public int GetRotationSteps(double degrees)
        {
            return Convert.ToInt32(4 * (5 * (degrees / 9)));
        }
    }
}
