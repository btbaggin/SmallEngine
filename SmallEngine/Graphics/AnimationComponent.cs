using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine.Graphics
{
    public class AnimationComponent : DependencyComponent, IUpdatable
    {
        public delegate void AnimationUpdateDelegate(AnimationComponent pComponent, ref Animation pCurrent);

        [ImportComponent]
        RenderComponent _render;
        readonly Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();

        Animation _current;
        public Animation Current
        {
            get { return _current; }
            private set { _current = value; }
        }

        public AnimationUpdateDelegate Evaluator { get; set; }
        
        public void Update(float pDeltaTime)
        {
            Evaluator?.Invoke(this, ref _current);

            Current.Update(pDeltaTime);

            _render.Bitmap = Current.Bitmap.CreateSubBitmap(Current.Frame);
        }

        public void AddAnimation(string pName, Animation pAnim)
        {
            _animations.Add(pName, pAnim);
        }

        public void AddDefaultAnimation(string pName, Animation pAnim)
        {
            _animations.Add(pName, pAnim);
            Current = pAnim;
            _render.Bitmap = Current.Bitmap.CreateSubBitmap(Current.Frame);
        }

        public Animation GetAnimation(string pName)
        {
            System.Diagnostics.Debug.Assert(_animations.ContainsKey(pName));
            return _animations[pName];
        }
    }
}
