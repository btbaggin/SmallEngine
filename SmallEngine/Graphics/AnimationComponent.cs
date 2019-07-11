using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Components;

namespace SmallEngine.Graphics
{
    [Serializable]
    public class AnimationComponent : BehaviorComponent
    {
        public delegate void AnimationUpdateDelegate(AnimationComponent pComponent, ref Animation pCurrent);

        [ImportComponent][NonSerialized]
        RenderComponent _render;

        [NonSerialized] Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();

        [NonSerialized] Animation _current;
        public Animation Current
        {
            get { return _current; }
            private set { _current = value; }
        }

        [field: NonSerialized]
        public AnimationUpdateDelegate Evaluator { get; set; }
        
        public override void Update(float pDeltaTime)
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

        [OnDeserializing]
        private void OnDeserializing(StreamingContext pContext)
        {
            _animations = new Dictionary<string, Animation>();
        }
    }
}
