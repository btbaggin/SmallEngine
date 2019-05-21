using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Graphics
{
    public class AnimationComponent : DependencyComponent, IUpdatable
    {
        [ImportComponent]
        RenderComponent _render;
        Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();

        public Animation Current { get; private set; }

        public Func<string> Evaluator { get; set; }

        public void Update(float pDeltaTime)
        {
            var anim = Evaluator.Invoke();
            System.Diagnostics.Debug.Assert(_animations.ContainsKey(anim));

            Current = _animations[anim];
            Current.Update(pDeltaTime);

            _render.Frame = Current.Frame;
            _render.Bitmap = Current.Bitmap;
        }

        public void AddAnimation(string pName, Animation pAnim)
        {
            _animations.Add(pName, pAnim);
        }

        public Animation GetAnimation(string pName)
        {
            System.Diagnostics.Debug.Assert(_animations.ContainsKey(pName));
            return _animations[pName];
        }
    }
}
