using Lib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace CarAI.AutoMotive
{
    public class Car
    {
        static Texture carTexture = new Texture("./assets/car.png");

        public Sprite Sprite { get; private set; }
        public float Angle { get; private set; } = 0f;
        public float Speed { get; private set; } = 1f;

        public CarGenotype Genotype { get; }

        public IReadOnlyList<Sensor> Sensors { get; private set; }

        public Car(Road road, CarGenotype genotype)
        {
            Genotype = genotype;
            Sprite = new Sprite(carTexture)
            {
                Origin = new Vector2f(carTexture.Size.X / 2, carTexture.Size.Y / 2),
                Position = new Vector2f(100, 50)
            };

            Sensors = new List<Sensor>() {
                new Sensor(road, this, new Vector2f(1, 0)),
                new Sensor(road, this, new Vector2f(0.5f, 0.2f)),
                new Sensor(road, this, new Vector2f(0.5f, -0.2f)),
                new Sensor(road, this, new Vector2f(0.5f, 0.6f)),
                new Sensor(road, this, new Vector2f(0.5f, -0.6f))
            }.AsReadOnly();
        }

        public double[] GetInputVector()
        {
            var values = new double[Sensors.Count];

            for (int i = 0; i < Sensors.Count; i++)
                values[i] = Sensors[i].Distance;

            return values;
        }

        public void Update()
        {
            foreach (var item in Sensors)
                item.Update();
        }

        public void Go(Direction dir)
        {
            if (dir == Direction.Left) Angle -= 3f;
            else if (dir == Direction.Right) Angle += 3f;

            Sprite.Rotation = Angle;
            MoveYourBody();
        }

        private void MoveYourBody()
        {
            var rad = MathHelper.GetRadian(Angle);
            var x = Math.Cos(rad);
            var y = Math.Sin(rad);

            Sprite.Position += new Vector2f((float)(x * Speed), (float)(y * Speed));
        }

        public void Draw(RenderWindow window)
        {
            /*
            var rect = Sprite.GetLocalBounds();
            var origin = Sprite.Origin;
            var leftTop = MathHelper.Rotate(new Vector2f(rect.Left, rect.Top), origin, Angle) + new Vector2f(Sprite.Position.X, Sprite.Position.Y) - Sprite.Origin;
            var rightTop = MathHelper.Rotate(new Vector2f(rect.Left + rect.Width, rect.Top), origin, Angle)+new Vector2f(Sprite.Position.X, Sprite.Position.Y)- Sprite.Origin ; ;
            var leftBottom = MathHelper.Rotate(new Vector2f(rect.Left, rect.Top + rect.Height), origin, Angle)+new Vector2f(Sprite.Position.X, Sprite.Position.Y)- Sprite.Origin ; ;
            var rightBottom = MathHelper.Rotate(new Vector2f(rect.Left + rect.Width, rect.Top + rect.Height), origin, Angle)+new Vector2f(Sprite.Position.X, Sprite.Position.Y)- Sprite.Origin; ;
            */
            window.Draw(Sprite);

            foreach (var item in Sensors)
                item.Draw(window);
            /*
            window.Draw(new Vertex[] { new Vertex(leftTop, Color.Red), new Vertex(rightTop, Color.Red) }, PrimitiveType.Lines);
            window.Draw(new Vertex[] { new Vertex(leftTop, Color.Red), new Vertex(leftBottom, Color.Red) }, PrimitiveType.Lines);
            window.Draw(new Vertex[] { new Vertex(leftBottom, Color.Red), new Vertex(rightBottom, Color.Red) }, PrimitiveType.Lines);
            window.Draw(new Vertex[] { new Vertex(rightTop, Color.Red), new Vertex(rightBottom, Color.Red) }, PrimitiveType.Lines);
            */
        }
    }
}
