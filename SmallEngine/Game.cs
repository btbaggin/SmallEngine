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

namespace SmallEngine
{
    public class Game : IDisposable
    {
        /*TODO
         * MessageBus dropping messages if too busy
         * Generic render component?
         */

        public enum RenderSystem
        {
            DirectX,
            OpenGL
        }

        private UIManager _uiManager;
        private SceneManager _sceneManager;
        private InputManager _inputManager;
        private float _timeElapsed;
        private int _frameCount;
        private static Random _random;

        #region Properties
        public static IGraphicsSystem Graphics { get; private set; }

        public int MaxFps { get; set; } = 0;

        public int CurrentFps { get; set; }

        public bool IsPlaying { get; set; }

        protected Scene Scene
        {
            get { return SceneManager.Current; }
        }

        public static GameForm Form { get; private set; }

        public static RenderSystem Render { get; set; }

        public bool PlayInBackground { get; set; }
        
        public bool Paused
        {
            get { return GameTime.Stopped; }
            set
            {
                if(value) { GameTime.Stop(); } else { GameTime.Start(); }
            }
        }

        public World GameWorld { get; private set; }

        public static Camera ActiveCamera { get; set; }
        #endregion  

        public Game()
        {
            Form = new GameForm();
            _uiManager = new UIManager();
            _sceneManager = new SceneManager(this);
            _inputManager = new InputManager(Form.Handle);
            GameWorld = new World();

            Form.WindowActivateChanged += WindowActivateChanged;
            Form.WindowDestory += WindowDestroyed;
            Form.WindowSizeChanged += WindowSizeChanged;

            Graphics = Render == RenderSystem.DirectX ? new DirectXGraphicSystem() : null;
            Graphics.Initialize(Form, false);

            _random = new Random();
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
            Scene.Update(pDeltaTime);
            ActiveCamera.Update(pDeltaTime);

            //Update game objects
            foreach (var go in Scene.GameObjects)
            {
                go.Update(pDeltaTime);
            }
            //Update physics
            GameWorld.Update(pDeltaTime);
        }

        private SortedSet<RenderComponent> _toRender = new SortedSet<RenderComponent>(new RenderComponentComparer());
        private void FilterVisible()
        {
            //Filter out ones not on screen
            _toRender.Clear();
            foreach(var r in RenderComponent.Renderers)
            {
                var onScreen = ActiveCamera.IsVisible(r.GameObject);
                if(onScreen && r.Visible && r.Opacity > 0f)
                {
                    _toRender.Add(r);
                }
            }        
        }

        private void Draw()
        {
            Scene.Draw(Graphics);

            foreach (var r in _toRender)
            {
                r.BeginDraw(Graphics);
                r.Draw(Graphics);
                r.EndDraw(Graphics);
            }

            _uiManager.Draw(Graphics);
        }
        #endregion

        internal void Destroy(IGameObject pGameObject)
        {
            Scene.Destroy(pGameObject);
        }

        public void Run()
        {
            IsPlaying = true;
            LoadContent();
            Initialize();
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
        }

        private void WindowDestroyed(object sender, WindowEventArgs e)
        {
            Exit();
        }

        private void WindowActivateChanged(object sender, WindowEventArgs e)
        {
            Paused = !e.Activated && !PlayInBackground;
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
                    SceneManager.EndScene();
                    UnloadContent();
                    Application.Exit();
                    return;
                }

                Scene.DisposeGameObjects();
                GameTime.Tick();

                //Cache pressed keys
                _inputManager.ProcessInput();

                Coroutine.Update(GameTime.DeltaTime);

                Update(GameTime.DeltaTime);
                FilterVisible();
                InputManager.MouseWheelDelta = 0;

                Graphics.BeginDraw();
                Draw();
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

        public static int RandomInt()
        {
            return _random.Next();
        }

        public static int RandomInt(int pMin, int pMax)
        {
            return _random.Next(pMin, pMax);
        }

        public static float RandomFloat()
        {
            return _random.NextFloat();
        }

        public static float RandomFloat(float pMin, float pMax)
        {
            return _random.Range(pMin, pMax);
        }

        public static double RandomDouble()
        {
            return _random.NextDouble();
        }

        public static double RandomDouble(double pMin, double pMax)
        {
            return pMin + (pMax - pMin) * _random.NextDouble();
        }
    }
}
