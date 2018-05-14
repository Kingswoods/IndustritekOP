using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Database.Models
{
    class Tag
    {
        public string UUID;
        public string TypeName;
        public decimal TypeCapacity;
        public int PosX;
        public int PosY;
        public int Rotation;

        public Tag(string id, string type, decimal capacity, int x, int y, int rot)
        {
            this.UUID = id;
            this.TypeName = type;
            this.TypeCapacity = capacity;
            this.PosX = x;
            this.PosY = y;
            this.Rotation = rot;
        }

        public Tag()
        {

        }

    }
}
