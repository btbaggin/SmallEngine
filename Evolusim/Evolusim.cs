using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Input;

namespace Evolusim
{
    class Evolusim : Game
    {

        public override void LoadContent()
        {
            //TODO load global bitmaps, audio, etc...
        }

        public override void Initialize()
        {
            SceneManager.BeginScene(new MenuScene());
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime); //Update GameObjects
        }
    }
}
