using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Audio;
using SmallEngine.Input;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Evolusim : Game
    {
        /*
         *TODO
         * Minimap
         * Creatures
         * Terrain bitmasking
         * Terrain climate
         * Only allow changing height / climate
         */

        public static int WorldSize { get { return 64 * Terrain.Size; } }

        public override void LoadContent()
        {
            ResourceManager.Add<BitmapResource>("water", "Graphics/water.png");
            ResourceManager.Add<BitmapResource>("plains", "Graphics/plains.png");
            ResourceManager.Add<BitmapResource>("mountain", "Graphics/mountain.jpg");
            ResourceManager.Add<BitmapResource>("desert", "Graphics/desert.jpg");
            ResourceManager.Add<BitmapResource>("forest", "Graphics/forest.jpg");
            ResourceManager.Add<BitmapResource>("organism", "Graphics/water.png");

            ResourceManager.Add<AudioResource>("menu", "Audio/misc_menu.wav");
        }

        public override void Initialize()
        {
            MaxFps = 120;
            Game.Form.Text = "Evolusim";
            Game.Form.Width = 1280;
            Game.Form.Height = 720;
            SceneManager.BeginScene(new MenuScene(false));

            InputManager.Listen(Keys.Left);
            InputManager.Listen(Keys.Right);
            InputManager.Listen(Keys.Up);
            InputManager.Listen(Keys.Down);

            ActiveCamera = new Camera(889, 4444, 500, 2500);
            ActiveCamera.Bounds = new System.Drawing.RectangleF(0, 0, WorldSize, WorldSize);
        }

        public override void Update(float pDeltaTime)
        {
            if(InputManager.KeyDown(Keys.Left))
            {
                ActiveCamera.MoveLeft();
            }

            if(InputManager.KeyDown(Keys.Right))
            {
                ActiveCamera.MoveRight();
            }

            if(InputManager.KeyDown(Keys.Up))
            {
                ActiveCamera.MoveUp();
            }

            if(InputManager.KeyDown(Keys.Down))
            {
                ActiveCamera.MoveDown();
            }

            base.Update(pDeltaTime);
        }
    }
}
