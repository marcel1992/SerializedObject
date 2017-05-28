using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SerializeObject_Example
{
    public class Program
    {
        private static void Main(string[] args)
        {

            using (var db = new SerializeContext())
            {
                var lstGestureClasses = new List<GestureClass>();

                //for (var i = 0; i < 10; i++)
                //{
                //    var positionVector1 = new Vector3(2.3f, 3.4f, 5.6f);
                //    lstGestureClasses.Add(
                //        new GestureClass
                //        {
                //            Time = DateTime.Now,
                //            //Position = positionVector1,
                //            Point3D = new List<Point3D>
                //            {
                //                new Point3D{X = 12,Y = 14,Z = 14,Joint = Point3D.JointType.HandRight},
                //                new Point3D{X = 13,Y = 15,Z = 16,Joint = Point3D.JointType.HandRight}

                //            },
                //            FormatedPosition = $"{positionVector1.X},{positionVector1.Y},{positionVector1.Z}"
                //        });


                //}

                Console.WriteLine("Save object:\n");

                #region save serialized object

                IFormatter formatter = new BinaryFormatter();
                foreach (var gesture in lstGestureClasses)
                {
                    using (var stream = new MemoryStream())
                    {
                        formatter.Serialize(stream, gesture);
                        var serializeObject = new TblSerializeObject { SerializedObject = stream.ToArray() };
                        db.TblSerializeObjects.Add(serializeObject);
                        db.SaveChanges();
                    }
                }

                #endregion

                Console.WriteLine("Read from database:\n");

                #region de-serialize

                var gestures = db.TblSerializeObjects.ToList();
                var contor = 0;
                foreach (var gesture in gestures)
                {
                    Stream readStream = new MemoryStream(gesture.SerializedObject);
                    var newGesture = (GestureClass) formatter.Deserialize(readStream);
                    //newGesture.Position = ExtractPosition(newGesture.FormatedPosition);
                    readStream.Close();
                    contor++;
                    Console.WriteLine(PrintGesture(newGesture, contor));

                }

                #endregion

                Console.Read();
            }

            //Vector3 ExtractPosition(string formatedPosition)
            //{

            //    var split = formatedPosition.Split(',');

            //    return new Vector3(
            //        float.Parse(split[0], CultureInfo.InvariantCulture),
            //        float.Parse(split[1], CultureInfo.InvariantCulture),
            //        float.Parse(split[2], CultureInfo.InvariantCulture));
            //}

            string PrintGesture(GestureClass gestureClass, int contor)
            {
                var points = string.Empty;
                if (gestureClass.Point3D != null)
                {
                    points = ExtractPoint3D(gestureClass.Point3D);
                }
                return $@"{contor}{":"}Points:{points}{" time:"}{gestureClass.Time}";
            }

            string ExtractPoint3D(IEnumerable<Point3D> lstPoint3D)
            {
                return lstPoint3D.Aggregate(string.Empty, (current, point3D) => current + $"{point3D.Joint} X: {point3D.X} Y: {point3D.Y} Z: {point3D.Z} ");
            }
        }
    }

    [Serializable]
    public class GestureClass
    {
        public DateTime Time { get; set; }
        //[NonSerialized] public Vector3 Position;
        public string FormatedPosition { get; set; }
        public List<Point3D> Point3D { get; set; }
    }

    [Serializable]
    public class Point3D
    {
        public double X, Y, Z;
        public JointType Joint { get; set; }
        public enum JointType
        {
            AnkleLeft,
            AnkleRight,
            ElbowLeft,
            ElbowRight,
            FootLeft,
            FootRight,
            HandLeft,
            HandRight,
            Head,
            HipCenter,
            HipLeft,
            HipRight,
            KneeLeft,
            KneeRight,
            ShoulderCenter,
            ShoulderLeft,
            ShoulderRight,
            Spine,
            WristLeft,
            WristRight
        }
    }
}