using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SmallEngine.Physics;

namespace SmallEngine.Debug
{
    static class DebugPrinter
    {
        class DebugSummary
        {
            public float Milliseconds;
            public long StartClock;
            public int Hits;
            public int HeaderIndex;
            public int Thread;

            public DebugSummary(long pStart, int pHeader, int pThread)
            {
                StartClock = pStart;
                HeaderIndex = pHeader;
                Thread = pThread;
            }
        }

        [Flags]
        public enum DisplayOptions
        {
            TimerText = 1,
            TimerGraph = 2,
            Colliders = 4,
            All = 65536
        }

        const int STORED_FRAMES = 120;

        public static DisplayOptions Display { get; set; } = DisplayOptions.All;
        static readonly Pen _pen = Pen.Create(Color.Aqua, 1);
        static readonly Font _font = Font.Create("Consolas", 14, Color.White);
        static readonly IEnumerable<DebugSummary>[] _pastFrames = new IEnumerable<DebugSummary>[STORED_FRAMES];
        static int _currentFrameIndex;

        [System.Diagnostics.Conditional("TRACE")]
        public static void Draw(IGraphicsAdapter pAdapter)
        {
            var summary = SummaryDebugLog();
            if(Display.HasFlag(DisplayOptions.TimerText) || Display == DisplayOptions.All)
            {
                RenderTimerText(pAdapter, summary);
            }
            if(Display.HasFlag(DisplayOptions.TimerGraph) || Display == DisplayOptions.All)
            {
                RenderTimerGraph(pAdapter);
            }
            if(Display.HasFlag(DisplayOptions.Colliders) || Display == DisplayOptions.All)
            {
                RenderColliders(pAdapter);
            }
        }

        private static IEnumerable<DebugSummary> SummaryDebugLog()
        {
            Dictionary<ulong, DebugSummary> summary = new Dictionary<ulong, DebugSummary>();

            //TODO this won't get things across frame boundaries
            var events = DebugLog.GetEvents(out uint count);
            for (int i = 0; i < count; i++)
            {
                var k = (ulong)(events[i].ThreadIndex << 31) | (ulong)events[i].HeaderIndex;
                switch (events[i].Type)
                {
                    case DebugLogTypes.Start:
                        if (!summary.ContainsKey(k)) summary.Add(k, new DebugSummary(events[i].Clock, events[i].HeaderIndex, events[i].ThreadIndex));
                        break;

                    case DebugLogTypes.End:
                        if (summary.ContainsKey(k))
                        {
                            summary[k].Milliseconds += GameTime.TickToMillis(events[i].Clock - summary[k].StartClock);
                            summary[k].Hits++;
                        }
                        break;

                    default:
                        throw new UnknownEnumException(typeof(DebugLogTypes), events[i].Type);
                }
            }

            _pastFrames[_currentFrameIndex] = summary.Values;
            _currentFrameIndex = (_currentFrameIndex + 1) % STORED_FRAMES;
            return summary.Values;
        }

        #region Timer Text
        private static void RenderTimerText(IGraphicsAdapter pAdapter, IEnumerable<DebugSummary> pSummary)
        {
            var y = 0f;
            foreach (var s in pSummary)
            {
                var header = DebugLog.Headers[s.HeaderIndex];

                var name = string.IsNullOrEmpty(header.Alias) ? header.Method : header.Alias;
                string print = $"{name}({header.Line}) {s.Milliseconds.ToString("F")}ms {s.Hits}h {(s.Milliseconds / s.Hits).ToString("F")}ms/h";
                pAdapter.DrawText(print, new Rectangle(0, y, Game.Form.Width, _font.Size), _font);
                y += _font.Size;
            }
        }
        #endregion

        #region Timer Graph
        private static void RenderTimerGraph(IGraphicsAdapter pAdpater)
        {
            const int GraphHeight = 300;
            const float MinFrameTime = 1000 / 30f;
            float BarWidth = Game.Form.Width / (float)STORED_FRAMES;

            byte[,] colors =
            {
                { 255, 255, 255 },
                { 255, 0,   0   },
                { 0,   255, 0   },
                { 0,   0,   255 },
                { 255, 0,   255 },
                { 255, 255, 0   },
                { 0,   255, 255 }
            };
            Brush[] brushes = new Brush[colors.GetLength(0)];
            for(int i = 0; i < brushes.Length; i++)
            {
                brushes[i] = SolidColorBrush.Create(new Color(colors[i, 0], colors[i, 1], colors[i, 2]));
            }

            var x = 0f;
            for(int i = 0; i < _pastFrames.Length; i++)
            {
                if(_pastFrames[i] != null)
                {
                    float y = Game.Form.Height;
                    int j = 0;
                    foreach(var s in _pastFrames[i])
                    {
                        var height = GraphHeight / MinFrameTime * s.Milliseconds;
                        var r = new Rectangle(x, y - height, BarWidth, height);
                        pAdpater.DrawRect(r, brushes[j % 10]);
                        if (r.Contains(Input.Mouse.Position))
                        {
                            var label_y = Math.Min(Input.Mouse.Position.Y, Game.Form.Height - _font.Size);
                            pAdpater.DrawText(DebugLog.Headers[s.HeaderIndex].Method, new Vector2(Input.Mouse.Position.X, label_y), _font); //TODO draw later?
                        }
                        y -= height;
                        j++;
                    }
                }
                x += BarWidth;
            }

            for(int i = 0; i < brushes.Length; i++) brushes[i].Dispose();
        }
        #endregion

        #region Colliders
        private static void RenderColliders(IGraphicsAdapter pAdapter)
        {
            var components = SmallEngine.Components.Component.GetComponentsOfType(typeof(ColliderComponent));
            foreach (var c in components)
            {
                var collider = (ColliderComponent)c;
                pAdapter.SetTransform(Transform.CreateBasic(collider.GameObject));
                switch (collider.Mesh.Shape)
                {
                    case Shapes.Circle:
                        var cir = (CircleMesh)collider.Mesh;
                        var center = Game.ActiveCamera.ToCameraSpace(collider.AABB.Center);
                        pAdapter.DrawElipseOutline(center, cir.Radius, _pen);
                        break;

                    case Shapes.Polygon:
                        var p = (PolygonMesh)collider.Mesh;
                        var pos = collider.AABB.Center;
                        Vector2 v1;
                        Vector2 v2;
                        for (int i = 0; i < p.Vertices.Length - 1; i++)
                        {
                            v1 = Game.ActiveCamera.ToCameraSpace(p.Vertices[i] + pos);
                            v2 = Game.ActiveCamera.ToCameraSpace(p.Vertices[i + 1] + pos);
                            pAdapter.DrawLine(v1, v2, _pen);
                        }
                        v1 = Game.ActiveCamera.ToCameraSpace(p.Vertices[p.Vertices.Length - 1] + pos);
                        v2 = Game.ActiveCamera.ToCameraSpace(p.Vertices[0] + pos);

                        pAdapter.DrawLine(v1, v2, _pen);
                        break;
                }
            }
        }
        #endregion
    }
}
