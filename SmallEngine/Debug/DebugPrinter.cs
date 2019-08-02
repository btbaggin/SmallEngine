using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.Debug
{
    static class DebugPrinter
    {
        [System.Diagnostics.Conditional("TRACE")]
        public static void Draw(IGraphicsAdapter pAdapter)
        {
            using (var font = Font.Create("Arial", 14, Color.White, pAdapter))
            {
                float y = 0;
                foreach (var record in TimedBlock._records.Values)
                {
                    record.GetHitCycleCount(out uint hit, out uint cycle);
                    float ms = GameTime.TickToMillis(cycle);
                    string print = $"{record.Method}({record.Line}) {ms.ToString("F")}ms {hit}h {(ms / hit).ToString("F")}ms/h";
                    pAdapter.DrawText(print, new Rectangle(0, y, Game.Form.Width, font.Size), font);
                    y += font.Size;
                }
            }
            //TOOD display stacked graph of input/update/render
        }
    }
}
