﻿using System;
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
        public class RenderComponentComparer : IComparer<IComponent>
        {
            public int Compare(IComponent x, IComponent y)
            {
                if (!(x is RenderComponent rc1) || !(y is RenderComponent rc2)) return 0;
                return rc1.ZIndex.CompareTo(rc2.ZIndex);
            }
        }

        [ImportComponent(false)][NonSerialized]
        Physics.RigidBodyComponent _body;

        public float Opacity { get; set; } = 1f;

        public int ZIndex { get; set; }

        public BitmapResource Bitmap { get; set; }

        [field: NonSerialized]
        public Effect Effect { get; set; } //TODO this needs to be serialized

        #region Constructor
        public RenderComponent() : base()
        {
            Comparer = new RenderComponentComparer();
        }

        public RenderComponent(BitmapResource pResource, int pZIndex) : this()
        {
            Bitmap = pResource;
            ZIndex = pZIndex;
        }

        protected override void OnDeserializeBegin()
        {
            base.OnDeserializeBegin();
            Comparer = new RenderComponentComparer();
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

                if (Effect != null) Effect.Draw(Bitmap, Opacity, position, scale);
                else pSystem.DrawBitmap(Bitmap, Opacity, position, scale);
            }
        }

        public bool IsVisible()
        {
            var onScreen = Game.ActiveCamera.IsVisible(GameObject);
            return onScreen && Opacity > 0f;
        }

        public override void Dispose()
        {
            base.Dispose();

            Bitmap?.Dispose();
            Effect?.Dispose();
        }
    }
}
