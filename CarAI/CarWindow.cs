using SFML.Graphics;
using SFML.Window;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CarAI
{
    class CarWindow
    {
        private RenderWindow window;

        public event EventHandler OnRender;
        public event EventHandler<KeyEventArgs> KeyDown;

        private EvolutionManager manager;

        public CarWindow(VideoMode mode, EvolutionManager manager)
        {
            this.manager = manager;
            window = new RenderWindow(mode, "Cars");
            window.SetFramerateLimit(200);

            window.KeyPressed += Window_KeyPressed;
            window.MouseButtonPressed += Window_MouseButtonPressed;
            window.MouseButtonReleased += Window_MouseButtonReleased;
            window.MouseMoved += Window_MouseMoved;
        }

        bool pressed = false;

        private void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (pressed)
                manager.Drag(e.X, e.Y);
        }

        private void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            pressed = false;
        }

        private void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            pressed = true;
        }

        private void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                window.Close();

            if (e.Code == Keyboard.Key.Space)
                manager.NewGeneration();

            if (e.Code == Keyboard.Key.R)
                manager.Reset();

            KeyDown?.Invoke(this, e);
        }

        internal void Start()
        {
            Core();
        }

        private void Core()
        {
            while (window.IsOpen)
            {
                manager.Update();
                window.Clear();
                window.DispatchEvents();
                OnRender?.Invoke(window, EventArgs.Empty);
                window.Display();
            }
        }
    }
}
