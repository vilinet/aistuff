using SFML.Graphics;
using SFML.System;
using System;

namespace CarAI.AutoMotive
{
    public class Sensor
    {
        public Vector2f Direction { get; set; }
        private Road Road { get; }
        private Car Car { get; }
        private Vector2f EndPoint { get; set; }
        public float Distance { get; private set; }

        public Sensor(Road road, Car car, Vector2f direction)
        {
            Road = road;
            Car = car;
            Direction = direction;
        }

        public void Update()
        {
            var endoint = Car.Sprite.Position;

            endoint.X += Direction.X * 100;
            endoint.Y += Direction.Y * 100;
            var rad = Car.Angle * 0.0174532925;
            double x = Direction.X * Math.Cos(rad) - Direction.Y * Math.Sin(rad);
            double y = Direction.Y * Math.Cos(rad) + Direction.X * Math.Sin(rad);
            EndPoint = Car.Sprite.Position + new Vector2f((float)(x * 1000), (float)(y * 1000));

            double minDist = 99999999999;
            foreach (var ring in Road.Rings)
            {
                for (uint i = 0; i < ring.Length; i++)
                {
                    var point = MathHelper.Intersects2D(Car.Sprite.Position, EndPoint, ring[i].Position, ring[(i + 1) % ring.Length].Position);

                    if (point != default )
                    {
                        var dist =(float)(Math.Sqrt(Math.Pow(point.X - Car.Sprite.Position.X, 2) + Math.Pow(point.Y - Car.Sprite.Position.Y, 2)));
                        if(dist < minDist)
                        {
                            EndPoint = point;
                            Distance = dist / 1000; //normalize top 700x700
                            minDist = dist;
                        }
                    }
                }
            }
        }

        public void Draw(RenderWindow window)
        {
           window.Draw(new Vertex[] { new Vertex(Car.Sprite.Position, Color.Yellow), new Vertex(EndPoint, Color.Yellow) }, PrimitiveType.Lines);
        }
    }
}
