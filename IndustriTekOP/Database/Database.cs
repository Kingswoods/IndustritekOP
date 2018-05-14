using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Database
{
    abstract class Database
    {
        public SQLiteConnection conn;
        public SQLiteCommand cmd;
        public SQLiteDataReader dtreader;

        public Database(string database)
        {
            conn = new SQLiteConnection("Data Source=" + database + ";Version=3;");

            Console.WriteLine(database);

            conn.Open();
            cmd = conn.CreateCommand();
        }

    }
}
