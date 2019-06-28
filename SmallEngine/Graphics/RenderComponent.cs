using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Input;
using SmallEngine.Components;
using System.Runtime.Serialization;

namespace SmallEngine.Graphics
{
    public sealed class RenderComponent : DependencyComponent
    {
        [ImportComponent(false)]
        Physics.RigidBodyComponent _body;

        public float Opacity { get; set; } = 1f;

        public int ZIndex { get; set; }

        public BitmapResource Bitmap { get; set; }

        #region Constructor
        public RenderComponent() : base() { }

        public RenderComponent(BitmapResource pResource, int pZIndex) : base()
        {
            Bitmap = pResource;
            ZIndex = pZIndex;
        }

        public RenderComponent(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
            Opacity = pInfo.GetSingle("Opacity");
            ZIndex = pInfo.GetInt32("ZIndex");
            Bitmap = ResourceManager.Request<BitmapResource>(pInfo.GetString("Bitmap"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Opacity", Opacity);
            info.AddValue("ZIndex", ZIndex);
            info.AddValue("Bitmap", Bitmap.Alias);
        }
        #endregion

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

                pSystem.DrawBitmap(Bitmap, Opacity, position, scale);
            }
        }

        public bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Opacity > 0f;
        }

        public override int CompareTo(IComponent other)
        {
            if (!(other is RenderComponent rc)) return 0;

            return ZIndex.CompareTo(rc.ZIndex);
        }

        public override void Dispose()
        {
            base.Dispose();

            Bitmap.Dispose();
        }
    }
}
