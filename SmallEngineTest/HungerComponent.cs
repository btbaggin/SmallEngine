using System.Collections.Generic;
using SmallEngine;

namespace SmallEngineTest
{
    class HungerComponent : Component
    {
        public bool SearchingForFood { get; private set; }

        private int _hunger;
        private int _currentHunger;
        private int _foodThreshold;
        private bool _alive;
        public HungerComponent(int pHunger, int pFoodThreshold)
        {
            _hunger = pHunger;
            _currentHunger = pHunger;
            _foodThreshold = pFoodThreshold;
            _alive = true;
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);
            Coroutine.Start(ConsumeHunger);
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            _alive = false;
        }

        public void Eat()
        {
            SearchingForFood = false;
            _currentHunger = _hunger;
        }

        private IEnumerator<WaitEvent> ConsumeHunger()
        {
            while(_alive)
            {
                _currentHunger -= 1;
                if(_currentHunger < _foodThreshold && _currentHunger > 0)
                {
                    SearchingForFood = true;
                }
                else if(_currentHunger <= 0)
                {
                    GameObject.Destroy();
                }
                else
                {
                    SearchingForFood = false;
                }
                yield return new WaitForSeconds(1f);
            }

        }
    }
}
