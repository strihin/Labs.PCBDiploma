using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace PCBDiploma
{
    public class Saving
    {
        public List<Cell> Points;
        public List<Cell> Walls;
        public List<int> Criteria;

        public Saving()
        {
            Points = new List<Cell>();
            Walls = new List<Cell>();
            Criteria = new List<int>();
        }
        public Saving(List<Cell> points, List<Cell>  walls, List<int> criteria)
        {
            
            Points = points;
            Walls = walls;
            Criteria = criteria;

        }

        public static Saving GetSaving()
        {
            Saving saving = null;
            string filename = Globals.SavingFile;

            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XmlSerializer xser = new XmlSerializer(typeof(Saving));
                    saving = (Saving)xser.Deserialize(fs);
                }
            }
            else saving = new Saving();
            return saving;

        }

        public void Save()
        {
            string fileName = Globals.SavingFile;
            if (File.Exists(fileName)) File.Delete(fileName);

            using (TextWriter writer = new StreamWriter(fileName))
            {
                XmlSerializer xser = new XmlSerializer(typeof(Saving));
                xser.Serialize(writer, this);
            }

        }
        private static void Serialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(Saving));
            TextWriter writer = new StreamWriter(filename);

            var save = new Saving
            {
        //         public List<Cell> Points;
        //public List<Cell> Walls;
        //public List<int> Criteria;
        //    Points=
        //        Points = new List<Point>
        //        {
        //            new Point{ Start = 1, End = 2},
        //            new Point{ Start = 2, End = 3}
        //        }
            };

            serializer.Serialize(writer, save);
            writer.Close();
        }
    }
}