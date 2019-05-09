using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using SmallEngine.Input;
using SmallEngine.Audio;
using SmallEngine.Graphics;
using SmallEngine.Physics;
using SmallEngine.UI;
using SmallEngine.Messages;

namespace SmallEngine
{
    public class Game : IDisposable
    {
        /*TODO
         * Generic render component?
         * Create a MessageBusPool so I can have many threads responding to things?
         */

        public enum RenderTypes
        {
            DirectX,
            OpenGL
        }

        readonly UIManager _uiManager;
        readonly RenderSystem _render;
        float _timeElapsed;
        int _frameCount;
        float _physicsStep;
        float _physicsStepTime;

        #region Properties
        public static Game Instance { get; private set; }

        public static IGraphicsAdapter Graphics { get; private set; }

        public static GameForm Form { get; private set; }

        public static RenderTypes Render { get; set; }

        public static MessageBus Messages { get; private set; }

        private int _maxFps;
        public int MaxFps
        {
            get { return _maxFps; }
            set
            {
                _maxFps = value;
                _physicsStepTime = 1f / _maxFps;
            }
        }

        public int CurrentFps { get; set; }

        public bool IsPlaying { get; set; }

        public bool PlayInBackground { get; set; }
        
        public bool Paused
        {
            get { return GameTime.Stopped; }
            set
            {
                if(value) { GameTime.Stop(); } else { GameTime.Start(); }
            }
        }

        public static Camera ActiveCamera { get; set; }
        #endregion  

        public Game()
        {
            Instance = this;
            Form = new GameForm();

            InputManager.Initialize(Form.Handle);
            Messages = new QueueingMessageBus();

            Form.WindowActivateChanged += WindowActivateChanged;
            Form.WindowDestory += WindowDestroyed;
            Form.WindowSizeChanged += WindowSizeChanged;

            Graphics = Render == RenderTypes.DirectX ? new DirectXAdapter() : null;
            Graphics.Initialize(Form, false);

            _uiManager = new UIManager();
            _render = new RenderSystem(Graphics);
            PhysicsHelper.Create();

            if (MaxFps == 0) MaxFps = 60;
        }

        #region Overridable game functions
        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void Update(float pDeltaTime)
        {
            _uiManager.Update(pDeltaTime);
            ActiveCamera.Update(pDeltaTime);

            Scene.UpdateAll(pDeltaTime);
        }

        private void Draw(float pDeltaTime)
        {
            _render.Update(pDeltaTime);
            _uiManager.Draw(Graphics);
        }
        #endregion

        public void Run()
        {
            IsPlaying = true;
            LoadContent();
            Initialize();
            PhysicsHelper.CreateQuadTree();
            Messages.Start();
            GameTime.Reset();

            Application.Idle += OnIdle;
            Application.Run(Form);
            Application.Idle -= OnIdle;
        }

        protected void Exit()
        {
            IsPlaying = false;
        }

        public void Dispose()
        {
            Graphics.Dispose();
            Form.Dispose();

            AudioPlayer.DisposePlayer();
        }

        private void WindowDestroyed(object sender, WindowEventArgs e)
        {
            Exit();
        }

        private void WindowActivateChanged(object sender, WindowEventArgs e)
        {
#if DEBUG
            if (!e.Activated) GameTime.TimeScale = .01f;
            else GameTime.TimeScale = 1f;
#else
            Paused = !e.Activated && !PlayInBackground;
#endif
        }

        private void WindowSizeChanged(object sender, WindowEventArgs e)
        {
            _uiManager.Resize();
        }

        private void OnIdle(object pSender, EventArgs pEventArgs)
        {
            while (!PeekMessage(out NativeMessage msg, IntPtr.Zero, 0, 0, 0))
            {
                if (!IsPlaying)
                {
                    Messages.Stop();
                    Scene.EndSceneAll();
                    UnloadContent();
                    Application.Exit();
                    return;
                }

                Scene.DisposeGameObjectsAll();
                GameTime.Tick();

                //Cache pressed keys
                InputManager.ProcessInput();

                Coroutine.Update(GameTime.DeltaTime);

                Update(GameTime.DeltaTime);

                _physicsStep += GameTime.DeltaTime;
                if (_physicsStep > .02f) _physicsStep = .02f;

                while (_physicsStep > _physicsStepTime)
                {
                    //Update physics
                    PhysicsHelper.Update(_physicsStepTime);
                    _physicsStep -= _physicsStepTime;
                }

                Graphics.BeginDraw();
                Draw(_physicsStep / _physicsStepTime);
                Graphics.EndDraw();

                //If the max updates is set and we have cycles, sleep the thread
                if (MaxFps > 0)
                {
                    var frameTime = GameTime.DeltaTime + GameTime.TickToMillis(GameTime.ElapsedSinceTick);
                    if (frameTime < 1000 / MaxFps)
                    {
                        var sleepTime = (int)(1000 / MaxFps - frameTime);
                        Thread.Sleep(sleepTime);
                    }
                }

                {//Calculate FPS
                    _frameCount++;
                    if ((_timeElapsed += GameTime.UnscaledDeltaTime) >= 1f)
                    {
                        CurrentFps = _frameCount;
                        _frameCount = 0;
                        _timeElapsed = 0;

                    }
                }
            }
        }      

#region PeekMessage PInvoke
        [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool PeekMessage(out NativeMessage pMsg, IntPtr pHWnd, uint pMessageFilterMin, uint pMessageFilterMax, uint pFlags);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }
#endregion
    }
}
