using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Audio;
using SmallEngine.Graphics;
using SmallEngine.Input;

using Evolusim.Terrain;

namespace Evolusim
{
    class Organism : GameObject
    {
        private int _health;
        public int Health
        {
            get { return _health; }
            set
            {
                _health = value;
                if (_health <= 0) Destroy();
            }
        }

        private AudioComponent _audio;
        private MovementComponent _movement;
        private StatusComponent _status;

        private TerrainType _preferredTerrain;

        public static Organism SelectedOrganism { get; set; }

        public override int Order => 10;

        #region Constructors
        static Organism()
        {
            SceneManager.Define("organism", typeof(AnimationRenderComponent),
                                            typeof(AudioComponent),
                                            typeof(TraitComponent),
                                            typeof(MovementComponent),
                                            typeof(StatusComponent),
                                            typeof(InspectionComponent));
        }

        public static Organism Create()
        {
            var go = SceneManager.Current.CreateGameObject<Organism>("organism");
            go.Tag = "Organism";
            return go;
        }

        public static Organism CreateFrom(Organism pOrgansihm, Organism pOrganism)
        {
            var go = SceneManager.Current.CreateGameObject<Organism>("organism");
            go.Tag = "Organism";
            go.Position = pOrganism.Position;
            return go;
        }

        public Organism()
        {
            Position = new Vector2(RandomGenerator.RandomInt(0, Evolusim.WorldSize), RandomGenerator.RandomInt(0, Evolusim.WorldSize));
            Scale = new Vector2(TerrainMap.BitmapSize);
            _preferredTerrain = TerrainMap.GetTerrainTypeAt(Position);
        }
        #endregion

        public override void Initialize()
        {
            var render = GetComponent<AnimationRenderComponent>();
            render.SetBitmap("organism");
            render.SetAnimation(4, new Vector2(16, 32), .5f, AnimationEval);

            _audio = GetComponent<AudioComponent>();
            _audio.SetAudio("nom");

            _movement = GetComponent<MovementComponent>();
            _status = GetComponent<StatusComponent>();

            MessageBus.Register(this);
        }

        public override void ReceiveMessage(GameMessage pM)
        {
            switch(pM.MessageType)
            {
                case "EnemySpawn":
                    var p = pM.GetValue<Vector2>();
                    if(Vector2.DistanceSqrd(p, Position) < (15 * 64) * (15 * 64))
                    {
                        _status.AddStatus(StatusComponent.Status.Scared);
                        Coroutine.Start(RunScared, p);
                    }
                    break;
            }
        }

        private IEnumerator<WaitEvent> RunScared(object pState)
        {
            var m = Vector2.MultiplyInDirection(100, Position, Position - (Vector2)pState);
            _movement.MoveTo(m, false);

            yield return new WaitForSeconds(2);//Run for 2 seconds
            _movement.MoveTo(Position, false);
            _status.RemoveStatus(StatusComponent.Status.Scared);
        }

        public void Eat(Vegetation pFood)
        {
            _movement.Stop(1f);
            _status.Eat(pFood);
            pFood.Destroy();
            _audio.PlayImmediate();
        }

        public void Mate(Organism pMate)
        {
            _movement.Stop(1f);
            CreateFrom(this, pMate);
            _status.Mate(pMate);
        }

        public IEnumerable<Tuple<string, float>> GetStats()
        {
            return _status.GetStats();
        }

        public void MoveTo(Vector2 pPosition)
        {
            _movement.MoveTo(pPosition, true);
        }
        
        private void AnimationEval(AnimationRenderComponent pComponent)
        {
            if (Math.Abs(_movement.Speed.X) - Math.Abs(_movement.Speed.Y) > 0)
            {
                pComponent.AnimationNum = _movement.Speed.X > 0 ? 3 : 1;
            }
            else
            {
                pComponent.AnimationNum = _movement.Speed.Y > 0 ? 2 : 0;
            }
        }
    }
}
