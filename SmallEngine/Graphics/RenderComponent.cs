using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Input;
using SmallEngine.Components;

namespace SmallEngine.Graphics
{
    public sealed class RenderComponent : DependencyComponent
    {
        [ImportComponent(false)]
        private Physics.RigidBodyComponent _body;

        public bool Visible { get; set; } = true;

        public float Opacity { get; set; } = 1f;

        public int ZIndex { get; set; }

        public BitmapResource Bitmap { get; set; }

        public Color Color { get; set; }

        public Effect Effect { get; set; }

        public void Draw(IGraphicsAdapter pSystem, float pDeltaTime)
        {
            if(Bitmap != null)
            {
                var t = Transform.Create(GameObject);
                pSystem.SetTransform(t);

                var position = GameObject.Position;
                if(_body != null) position += _body.Velocity * pDeltaTime * GameTime.DeltaTime; //Interpolate position based on the amount of leftover update time
                position = Game.ActiveCamera.ToCameraSpace(position);

                var scale = GameObject.Scale * Game.ActiveCamera.Zoom;

                if (Effect != null)
                    Effect.Draw(Bitmap, position);
                else pSystem.DrawBitmap(Bitmap, Opacity, position, scale);
            }
        }

        public bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Visible && Opacity > 0f;
        }

        public override int CompareTo(IComponent other)
        {
            if (!(other is RenderComponent rc)) return 0;

            return ZIndex.CompareTo(rc.ZIndex);
        }
    }
}
