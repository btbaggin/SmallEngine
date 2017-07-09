using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Input;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Evolusim : Game
    {
        /*
         *TODO
         * Camera to move around/zoom
         */

        public static Camera MainCamera { get; private set; }

        public override void LoadContent()
        {
            ResourceManager.Add<BitmapResource>("water", "Graphics/water.png");
            ResourceManager.Add<BitmapResource>("plains", "Graphics/plains.png");
            ResourceManager.Add<BitmapResource>("mountain", "Graphics/mountain.jpg");
            ResourceManager.Add<BitmapResource>("desert", "Graphics/desert.jpg");
            ResourceManager.Add<BitmapResource>("forest", "Graphics/forest.jpg");
        }

        public override void Initialize()
        {
            Game.Form.Text = "Evolusim";
            Game.Form.Width = 1280;
            Game.Form.Height = 720;
            SceneManager.BeginScene(new MenuScene(false));

            InputManager.Listen(Keys.Left);
            InputManager.Listen(Keys.Right);
            InputManager.Listen(Keys.Up);
            InputManager.Listen(Keys.Down);

            MainCamera = new Camera(500, 2500, 500, 2500);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime); //Update GameObjects
            
            if(InputManager.KeyDown(Keys.Left))
            {
                MainCamera.MoveLeft();
            }

            if(InputManager.KeyDown(Keys.Right))
            {
                MainCamera.MoveRight();
            }

            if(InputManager.KeyDown(Keys.Up))
            {
                MainCamera.MoveUp();
            }

            if(InputManager.KeyDown(Keys.Down))
            {
                MainCamera.MoveDown();
            }

            MainCamera.Update(pDeltaTime);
        }
    }
}
