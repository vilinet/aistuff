using CarAI.AutoMotive;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarAI
{
   public class Road
    {
        public Vertex[][] Rings { get; private set; }
        public Vertex[] Objects { get; private set; }
        public Road()
        {
            Rings = LoadVertexes("./assets/map");
           
        }

        private Vertex[][] LoadVertexes(string file)
        {
            var lines = File.ReadAllLines("./assets/map");
            int count = lines.Count(x => x == "==");

            var rings = new Vertex[count][];
            List<Vertex> list = new List<Vertex>(); ;
            int ringId = 0;
            foreach (var line in lines)
            {
                if (line == "==")
                {
                    rings[ringId++] = list.ToArray();
                    list = new List<Vertex>();
                }
                else
                {
                    if (ringId > 1)
                    {
                        var parts = line.Split(",");
                        var vertex = new Vertex(new Vector2f(int.Parse(parts[0]), int.Parse(parts[1])), Color.Red);
                        list.Add(vertex);
                    }
                    else
                    {
                        var parts = line.Split(",");
                        var vertex = new Vertex(new Vector2f(int.Parse(parts[0]), int.Parse(parts[1])), Color.Blue);
                        if (list.Count > 0)
                        {
                            list.Add(new Vertex((list[list.Count - 1].Position + vertex.Position) / 2, Color.Cyan));
                        }
                        list.Add(vertex);
                    }
                }
            }
            return rings;
        }

        public bool Intersects(Car car)
        {
            var rect = car.Sprite.GetLocalBounds();
            var origin = car.Sprite.Origin;
            var leftTop = MathHelper.Rotate(new Vector2f(rect.Left, rect.Top), origin, car.Angle) + new Vector2f(car.Sprite.Position.X, car.Sprite.Position.Y) - car.Sprite.Origin;
            var rightTop = MathHelper.Rotate(new Vector2f(rect.Left + rect.Width, rect.Top), origin, car.Angle) + new Vector2f(car.Sprite.Position.X, car.Sprite.Position.Y) - car.Sprite.Origin; ;
            var leftBottom = MathHelper.Rotate(new Vector2f(rect.Left, rect.Top + rect.Height), origin, car.Angle) + new Vector2f(car.Sprite.Position.X, car.Sprite.Position.Y) - car.Sprite.Origin; ;
            var rightBottom = MathHelper.Rotate(new Vector2f(rect.Left + rect.Width, rect.Top + rect.Height), origin, car.Angle) + new Vector2f(car.Sprite.Position.X, car.Sprite.Position.Y) - car.Sprite.Origin; ;

            foreach (var ring in Rings)
            {
                for (uint i = 0; i < ring.Length; i++)
                {
                    var a = ring[i].Position;
                    var b = ring[(i+1)%ring.Length].Position;

                    if (MathHelper.Intersects2D(a,b, leftTop, rightTop) != default ||
                        MathHelper.Intersects2D(a, b, leftTop, leftBottom) != default ||
                        MathHelper.Intersects2D(a, b, rightTop, rightBottom) != default ||
                        MathHelper.Intersects2D(a, b, leftBottom, rightBottom) != default)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Draw(RenderWindow window)
        {
            foreach (var ring in Rings)
            {
                window.Draw(ring, PrimitiveType.LineStrip);
                window.Draw(new Vertex[] { ring[ring.Length - 1], ring[0] }, PrimitiveType.Lines);
            }

            for (uint i = 0; i < Math.Min(Rings[0].Length, Rings[1].Length); i++)
            {
                window.Draw(new Vertex[] { Rings[0][i], Rings[1][i] }, PrimitiveType.Lines);
            }
        }

        public int Checkpoint(Car car) {
            var rect = car.Sprite.GetGlobalBounds();

            var leftTop = new Vector2f(rect.Left, rect.Top);
            var rightTop = new Vector2f(rect.Left + rect.Width, rect.Top);
            var leftBottom = new Vector2f(rect.Left, rect.Top + rect.Height);
            var rightBottom = new Vector2f(rect.Left + rect.Width, rect.Top + rect.Height);
          
            for (uint i = 0; i < Math.Min(Rings[0].Length, Rings[1].Length); i++)
            {
                var a = Rings[0][i].Position;
                var b = Rings[1][i].Position;
                if (MathHelper.Intersects2D(a, b, leftTop, rightTop) != default ||
                   MathHelper.Intersects2D(a, b, leftTop, leftBottom) != default ||
                   MathHelper.Intersects2D(a, b, rightTop, rightBottom) != default ||
                   MathHelper.Intersects2D(a, b, leftBottom, rightBottom) != default)
                {
                    return ((int)i)+1;
                }
            }
            return -1;
        }

        public int GetMaxCheckpoint()
        {
            return Rings[0].Length + 1;
        }

      
    }
}
