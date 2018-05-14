using IndustriTekOP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Database.Tables
{
    class Tags : Database
    {
        public Tags(string database) : base(database)
        {

        }

        public Tag GetTagByUUID(string UUID)
        {
            this.cmd.CommandText = "SELECT UUID, TypeName, TypeCapacity, PosXValue, PosYValue, PosOrientation FROM Tags INNER JOIN Types ON Tags.TypeID = Types.TypeID INNER JOIN Positions ON Types.PositionID = Positions.PositionID WHERE UUID = '" + UUID + "'" ;

            this.dtreader = this.cmd.ExecuteReader();

            if (!this.dtreader.HasRows)
            {
                this.dtreader.Close();

                this.cmd.CommandText = "SELECT TypeName, TypeCapacity, PosXValue, PosYValue, PosOrientation FROM Types INNER JOIN Positions ON Types.PositionID = Positions.PositionID WHERE TypeName = 'Unknown'";

                this.dtreader = this.cmd.ExecuteReader();

                while(this.dtreader.Read())
                {
                    Tag tag = new Tag(UUID, Convert.ToString(this.dtreader["TypeName"]), Convert.ToDecimal(this.dtreader["TypeCapacity"]), Convert.ToInt32(this.dtreader["PosXValue"]), Convert.ToInt32(this.dtreader["PosYValue"]), Convert.ToInt32(this.dtreader["PosOrientation"]));

                    return tag;
                }
            }
            else
            {
                while (this.dtreader.Read())
                {
                    Tag tag = new Tag(Convert.ToString(this.dtreader["UUID"]), Convert.ToString(this.dtreader["TypeName"]), Convert.ToDecimal(this.dtreader["TypeCapacity"]), Convert.ToInt32(this.dtreader["PosXValue"]), Convert.ToInt32(this.dtreader["PosYValue"]), Convert.ToInt32(this.dtreader["PosOrientation"]));

                    return tag;
                }
            }

            //Return empty tag just in case.
            Tag emptyTag = new Tag();

            return emptyTag;

        }

        public Tag GetTagByTypeName(string typeName)
        {
            this.cmd.CommandText = "SELECT TypeName, TypeCapacity, PosXValue, PosYValue, PosOrientation FROM Types INNER JOIN Positions ON Types.PositionID = Positions.PositionID WHERE TypeName = '" + typeName + "'";

            this.dtreader = this.cmd.ExecuteReader();

            while (this.dtreader.Read())
            {
                Tag tag = new Tag(null, Convert.ToString(this.dtreader["TypeName"]), Convert.ToDecimal(this.dtreader["TypeCapacity"]), Convert.ToInt32(this.dtreader["PosXValue"]), Convert.ToInt32(this.dtreader["PosYValue"]), Convert.ToInt32(this.dtreader["PosOrientation"]));

                return tag;
            }

            //Return empty tag just in case.
            Tag emptyTag = new Tag();

            return emptyTag;
        }
    }
}
