using Lib.AI;
using Lib.Evolution;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CarAI
{
    class Program
    {
        private static EvolutionManager manager;

        static void Main(string[] args)
        {
            manager = new EvolutionManager();

            var window = new CarWindow(new VideoMode(700, 700), manager);
            window.OnRender += Window_OnRender;
            window.Start();
        }

        private static void Window_OnRender(object sender, EventArgs e)
        {
            Draw(sender as RenderWindow);
        }

        private static void Draw(RenderWindow window)
        {
            manager.Road.Draw(window);

            foreach (var item in manager.Cars)
            {
                item.Draw(window);
            }
        }
    }
}
