using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarAI
{
    public static class MathHelper
    {
        public static Vector2f Intersects2D(Vector2f l1s, Vector2f l1e, Vector2f l2s, Vector2f l2e)
        {
            double firstLineSlopeX = l1e.X - l1s.X;
            double firstLineSlopeY = l1e.Y - l1s.Y;

            double secondLineSlopeX = l2e.X - l2s.X;
            double secondLineSlopeY = l2e.Y - l2s.Y;

            double s = (-firstLineSlopeY * (l1s.X - l2s.X) + firstLineSlopeX * (l1s.Y - l2s.Y)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
            double t = (secondLineSlopeX * (l1s.Y - l2s.Y) - secondLineSlopeY * (l1s.X - l2s.X)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                float intersectionPointX = (float)(l1s.X + (t * firstLineSlopeX));
                float intersectionPointY = (float)(l1s.Y + (t * firstLineSlopeY));

                var intersect = new Vector2f(intersectionPointX, intersectionPointY);
                return intersect;
            }

            return default;
        }

        public static double GetRadian(double degree)
        {
            return degree * 0.0174532925;
        }

        public static Vector2f Rotate(Vector2f point, Vector2f center, double angle)
        {
            angle = GetRadian(angle);
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            var rotatedX = cos * (point.X - center.X) - sin * (point.Y - center.Y) + center.X;
            var rotatedY = sin * (point.X - center.X) + cos * (point.Y - center.Y) + center.Y;

            return new Vector2f((float)rotatedX, (float)rotatedY);
        }
    }
}
