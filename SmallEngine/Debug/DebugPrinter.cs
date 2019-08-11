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
        #region Helper class
        class DebugSummary
        {
            public float Milliseconds;
            public int Hits;
            public int HeaderIndex;
            public int Thread;
            public List<DebugSummary> Children = new List<DebugSummary>();

            public DebugSummary(int pHeader, int pThread)
            {
                HeaderIndex = pHeader;
                Thread = pThread;
            }

            public DebugSummary(int pHeader, int pThread, float pMilliseconds)
            {
                HeaderIndex = pHeader;
                Thread = pThread;
                Milliseconds = pMilliseconds;
                Hits = 1;
            }
        }
        #endregion

        #region Properties
        public static bool Paused { get; set; }

        public static DisplayOptions Display { get; set; } = DisplayOptions.All;
        #endregion

        const int STORED_FRAMES = 120;
        const int GRAPH_HEIGHT = 300;
        const float MIN_FRAME_TIME = 1000 / 30f;

        static readonly Pen _pen = Pen.Create(Color.Aqua, 1);
        static readonly Font _font = Font.Create("Consolas", 14, Color.White);
        static readonly DebugSummary[][] _pastFrames = new DebugSummary[STORED_FRAMES][];
        static int _currentFrameIndex;
        static int _selectedFrameIndex = -1;
        static readonly Brush[] _brushes = new Brush[]
        {
            SolidColorBrush.Create(new Color(255, 255, 255)),
            SolidColorBrush.Create(new Color(255, 0, 0)),
            SolidColorBrush.Create(new Color(0, 255, 0)),
            SolidColorBrush.Create(new Color(0, 0, 255)),
            SolidColorBrush.Create(new Color(255, 0, 255)),
            SolidColorBrush.Create(new Color(255, 255, 0)),
            SolidColorBrush.Create(new Color(0, 255, 255))
        };

        [System.Diagnostics.Conditional("TRACE")]
        public static void Draw(IGraphicsAdapter pAdapter)
        {
            var events = DebugLog.GetEvents(out uint count);
            var summary = SummaryDebugLog(events, count);
            if (Display.HasFlag(DisplayOptions.TimerText)) RenderTimerText(pAdapter, Vector2.Zero, summary);

            if(Display.HasFlag(DisplayOptions.TimerGraph)) RenderTimerGraph(pAdapter);

            if (Display.HasFlag(DisplayOptions.Messages)) RenderMessages(pAdapter, events, count);

            if(Display.HasFlag(DisplayOptions.Colliders)) RenderColliders(pAdapter);

            if (_selectedFrameIndex != -1)
            {
                var x = Game.Form.Width / 2f;
                RenderTimerText(pAdapter, new Vector2(x, 0), _pastFrames[_selectedFrameIndex]);
            }
        }

        private static DebugSummary[] SummaryDebugLog(DebugLogEvent[] pEvents, uint pCount)
        {
            //TODO this won't get things across frame boundaries

            //Determine the end indexes for each start event.
            //This allows us to more easily walk the array later
            int[] end_indexes = new int[pCount];
            Dictionary<ulong, int> summary = new Dictionary<ulong, int>();
            for(int i= 0; i < pCount; i++)
            {
                var k = (ulong)(pEvents[i].ThreadIndex << 31) | (ulong)pEvents[i].HeaderIndex;
                var type = pEvents[i].Type;

                if(type == DebugLogTypes.Start) summary[k] = i;
                else if (type == DebugLogTypes.End) end_indexes[summary[k]] = i;
            }

            DebugSummary[] summeries = new DebugSummary[DebugLog.HeaderCount];
            for (int i = 0; i < pCount; i++)
            {
                var header = pEvents[i].HeaderIndex;
                var thread = pEvents[i].ThreadIndex;

                //If this is a start node
                if (end_indexes[i] != 0)
                {
                    var start_clock = pEvents[i].Clock;
                    var end_clock = pEvents[end_indexes[i]].Clock;

                    //Add a root node that marks the total time for the method
                    if (summeries[header] == null) summeries[header] = new DebugSummary(header, thread);
                    summeries[header].Milliseconds += GameTime.TickToMillis(end_clock - start_clock);
                    summeries[header].Hits++;

                    var currentNode = summeries[header];
                    BuildChildren(currentNode, i);
                }
            }

            if(!Paused)
            {
                //Save the frame
                _pastFrames[_currentFrameIndex] = summeries;
                _currentFrameIndex = (_currentFrameIndex + 1) % STORED_FRAMES;
                if (_currentFrameIndex == _selectedFrameIndex) _selectedFrameIndex = -1;
            }
            return summeries;

            void BuildChildren(DebugSummary pCurrent, int pIndex)
            {
                for (int j = pIndex + 1; j < end_indexes[pIndex]; j++)
                {
                    if (end_indexes[j] != 0)
                    {
                        var nextNode = new DebugSummary(pEvents[j].HeaderIndex, pEvents[j].ThreadIndex, GameTime.TickToMillis(pEvents[end_indexes[j]].Clock - pEvents[j].Clock));

                        pCurrent.Children.Add(nextNode);
                        BuildChildren(nextNode, j);

                        //All nodes between j and end_indexs[j] would be covered by the children
                        j = end_indexes[j];
                    }
                }
            }
        }

        #region Timer Text
        private static void RenderTimerText(IGraphicsAdapter pAdapter, Vector2 pStart, DebugSummary[] pSummary)
        {
            var y = 0f;
            for(int i = 0; i < pSummary.Length; i++)
            {
                var header = DebugLog.Headers[i];
                var name = string.IsNullOrEmpty(header.Alias) ? header.Method : header.Alias;
                var s = pSummary[i];

                float ms = 0;
                int hits = 0;
                if (s != null)
                {
                    ms = s.Milliseconds;
                    hits = s.Hits;
                }

                var n = name.PadLeft(20, ' ');
                var l = header.Line.ToString().PadLeft(4, ' ');
                var t = ms.ToString("F").PadLeft(5, ' ');
                var h = hits.ToString().PadLeft(3, ' ');
                var th = (ms / hits).ToString("F").PadLeft(5, ' ');
                string print = $"{n}({l}) {t}ms {h}h {th}ms/h";
                pAdapter.DrawText(print, new Rectangle(pStart.X, pStart.Y + y, Game.Form.Width, _font.Size), _font);
                y += _font.Size;
            }
        }
        #endregion

        #region Timer Graph
        private static void RenderTimerGraph(IGraphicsAdapter pAdapter)
        {
            float BarWidth = Game.Form.Width / (float)STORED_FRAMES;

            int headerToRenderIndex = -1;
            var x = 0f;
            for (int i = 0; i < _pastFrames.Length; i++)
            {
                if (_pastFrames[i] != null)
                {
                    float y = Game.Form.Height;
                    for (int j = 0; j < _pastFrames[i].Length; j++)
                    {
                        //One of these could be null if we never called that method on this frame
                        if (_pastFrames[i][j] != null)
                        {
                            DrawRect(_pastFrames[i][j], j, ref y);

                            var p = Input.Mouse.Position;
                            if(x <= p.X && y <= p.Y && x + BarWidth >= p.X && Game.Form.Height >= p.Y && 
                               Input.Mouse.ButtonPressed(Input.MouseButtons.Left))
                            {
                                _selectedFrameIndex = i;
                                Paused = true;
                            }
                        }
                    }
                }
                x += BarWidth;
            }

            if(headerToRenderIndex != -1)
            {
                var header = DebugLog.Headers[headerToRenderIndex];
                var label_y = Math.Min(Input.Mouse.Position.Y, Game.Form.Height - _font.Size);
                var label_text = string.IsNullOrEmpty(header.Alias) ? header.Method : header.Alias;
                pAdapter.DrawText(label_text, new Vector2(Input.Mouse.Position.X, label_y), _font);
            }


            bool DrawRect(DebugSummary pSummary, int pBrushIndex, ref float pY)
            {
                float height = 0;
                if (pSummary.Children.Count == 0)
                {
                    height = GRAPH_HEIGHT / MIN_FRAME_TIME * pSummary.Milliseconds;
                }
                else
                {
                    //If we have children we need to subtract their total time from our time
                    //to get the time that was spend only in this method
                    var ms = pSummary.Milliseconds - pSummary.Children.Sum((pC) => pC.Milliseconds);
                    height = GRAPH_HEIGHT / MIN_FRAME_TIME * ms;
                }

                var r = new Rectangle(x, pY - height, BarWidth, height);
                var b = _brushes[pBrushIndex % _brushes.Length];
                pAdapter.DrawRect(r, b);

                var mouseOver = r.Contains(Input.Mouse.Position);
                if (mouseOver) headerToRenderIndex = pSummary.HeaderIndex;
                pY -= height;

                for(int i = 0; i < pSummary.Children.Count; i++)
                {
                    //TODO combine same children into one rectangle?
                    mouseOver |= DrawRect(pSummary.Children[i], pBrushIndex + i, ref pY);
                }

                return mouseOver;
            }
        }
        #endregion

        #region Messages
        private static void RenderMessages(IGraphicsAdapter pAdapter, DebugLogEvent[] pEvents, uint pCount)
        {
            for(int i = 0; i < pCount; i++)
            {
                if(pEvents[i].Type == DebugLogTypes.Message)
                {
                    //TODO display for X seconds
                    //TODO better position
                    pAdapter.DrawText(DebugMessage.GetMessage(i), new Vector2(100, 100), _font); 
                }
            }
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
