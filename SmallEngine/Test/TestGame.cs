using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Test
{
    class TestGame : Game
    {
        Swarm _swarm;

        public override void Initialize()
        {
            //Form.FullScreen = true;
            PlayInBackground = false;
            Form.Width = 640;
            Form.Height = 480;

            Input.InputManager.AddMapping("spread", Input.Keys.Space, 1000);
            Input.InputManager.AddMapping("stuff", Input.Mouse.Left);
            Input.InputManager.AddMapping("exit", Input.Keys.Escape);
            _currentState = Input.InputManager.GetInputState();
            _previousState = Input.InputManager.GetInputState();

            _swarm = new Swarm(100) { Persistant = true };
            SceneManager.BeginScene("Boids");
            SceneManager.AddToScene(_swarm);
        }

        public override void LoadContent()
        {
            ResourceManager.SetDirectory(typeof(Audio.AudioResource), "Audio");
            ResourceManager.SetDirectory(typeof(Graphics.BitmapResource), "Graphics");
            ResourceManager.Add<Graphics.BitmapResource>("bitmap", "image 1.jpg");
            ResourceManager.Add<Graphics.BitmapResource>("boid", "bird.png");
            ResourceManager.Add<Audio.AudioResource>("applause", "applause.wav");
            Graphics.DefineColor(System.Drawing.Color.White);
            Graphics.DefineColor(System.Drawing.Color.Blue);
        }

        public IEnumerator<WaitEvent> Spread()
        {
            Boid.Spread = true;
            yield return new WaitForSeconds(1f);
            Boid.Spread = false;
        }

        Input.InputState _previousState;
        Input.InputState _currentState;
        public override void Update(float pDeltaTime)
        {
            _currentState = Input.InputManager.GetInputState();
            if (_currentState.IsPressed("spread") && !_previousState.IsPressed("spread"))
            {
                Coroutine.Start(Spread);
            }

            if(_currentState.IsPressed("exit"))
            {
                Exit();
            }

            if(_currentState.IsPressed("stuff") && !_previousState.IsPressed("stuff"))
            {
                var a = Input.InputManager.MousePosition;
            }

            Form.Text = CurrentFps.ToString();
            _previousState = _currentState;
            base.Update(pDeltaTime);
        }
    }
}
