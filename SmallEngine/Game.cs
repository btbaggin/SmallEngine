//#define FPS

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
    public class Game : IUpdatable, IDisposable
    {
#if FPS
        float _timeElapsed;
        int _frameCount;
#endif
        readonly BehaviorSystem _update;
        readonly RenderSystem _render;
        float _physicsStep;

#region Properties
        public static Game Instance { get; private set; }

        public static GameForm Form { get; private set; }

        public static IGraphicsAdapter Graphics { get; private set; }

        public static RenderMethods RenderMethod { get; set; }

        private int _maxFps;
        public int MaxFps
        {
            get { return _maxFps; }
            set
            {
                _maxFps = value;
                GameTime.PhysicsTime = 1f / _maxFps;
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

            Mouse.SetHandle(Form.Handle);

            var availableThreads = MessageThreads;
            Messages = new QueueingMessageBus(availableThreads); 

            Form.WindowActivateChanged += WindowActivateChanged;
            Form.WindowDestory += WindowDestroyed;
            Form.WindowSizeChanged += WindowSizeChanged;

            Graphics = GraphicsAdapterFactory.Create(RenderMethod);
            if(!Graphics.Initialize(Form, false))
                throw new InvalidOperationException("Unable to initialize graphics adapter. View debug window for details.");

            Form.WindowSizeChanged += Graphics.Resize;

            _render = new RenderSystem(Graphics);
            _update = new BehaviorSystem();
            PhysicsHelper.Create();

            ActiveCamera = new Camera(1f, 1f)
            {
                Width = Form.Width,
                Height = Form.Height
            };

            if (MaxFps == 0) MaxFps = 60;
        }

#region Overridable game functions
        public virtual void Initialize() { }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Update(float pDeltaTime)
        {
            ActiveCamera.Update(pDeltaTime);

            Scene.UpdateUI();
            _update.Update(pDeltaTime);
            Scene.UpdateAll(pDeltaTime); //TODO Remove eventually?
        }

        private void Draw()
        {
            _render.Process();
            Scene.DrawAll(Graphics); //TODO remove eventually
            _update.Draw(Graphics);
            Scene.DrawUI(Graphics);
        }
#endregion

        public void Run()
        {
            IsPlaying = true;
            Messages.Start();
            GameTime.Reset();

            LoadContent();
            Initialize();

            PhysicsHelper.CreateQuadTree();

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

#region Messages
        public static int MessageThreads { get; set; } = System.Environment.ProcessorCount - 1; //reserve dedicated thread for audio

        public static MessageBus Messages { get; set; }

        public static void SetMessageThreads(int pThreads)
        {
            if (pThreads <= 0) throw new ArgumentException(nameof(pThreads));
            MessageThreads = pThreads;
        }
#endregion

#region Event Handlers
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
            Scene.InvalidateAllMeasure();
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

                GameTime.Tick();

                var physicsTime = GameTime.PhysicsTime;
                var deltaTime = GameTime.DeltaTime;

                //Cache pressed keys
                var input = Keyboard.GetInput();
                Keyboard.SetState(input);
                Mouse.SetState(input);

                Coroutine.Update(deltaTime);

                Update(deltaTime);

                _physicsStep += deltaTime;
                if (_physicsStep > .02f) _physicsStep = .02f;

                while (_physicsStep > physicsTime)
                {
                    //Update physics
                    PhysicsHelper.Update();
                    _physicsStep -= physicsTime;
                }

                Graphics.BeginDraw();

                GameTime.RenderTime = _physicsStep / physicsTime;
                Draw();

                Graphics.EndDraw();

                Scene.DisposeDestroyedGameObjects();
                Mouse.WheelDelta = 0;

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

#if FPS
                {//Calculate FPS
                    _frameCount++;
                    if ((_timeElapsed += GameTime.UnscaledDeltaTime) >= 1f)
                    {
                        CurrentFps = _frameCount;
                        _frameCount = 0;
                        _timeElapsed = 0;

                    }
                }
#endif
            }
        }
#endregion

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
