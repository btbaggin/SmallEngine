﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Audio;
using SmallEngine.Input;
using SmallEngine.Graphics;

using Evolusim.Terrain;

namespace Evolusim
{
    class Evolusim : Game
    {
        /*
         *TODO
         * Terrain bitmasking
         * Only allow changing height / climate
         */

        public static int WorldSize { get { return 64 * TerrainMap.Size; } }

        public override void LoadContent()
        {
            ResourceManager.Add<BitmapResource>("organism", "Graphics/organism.png", true);
            ResourceManager.Add<BitmapResource>("hungry", "Graphics/hungry.png", true);
            ResourceManager.Add<BitmapResource>("heart", "Graphics/heart.png", true);

            ResourceManager.Add<BitmapResource>("v_water", "Graphics/v_water.png", true);
            ResourceManager.Add<BitmapResource>("v_grassland", "Graphics/v_grassland.png", true);
            ResourceManager.Add<BitmapResource>("v_shrubland", "Graphics/v_shrubland.png", true);
            ResourceManager.Add<BitmapResource>("v_temperatedeciduous", "Graphics/v_temperatedeciduous", true);
            ResourceManager.Add<BitmapResource>("v_temperatedesert", "Graphics/v_temperatedesert", true);
            ResourceManager.Add<BitmapResource>("v_subtropicaldesert", "Graphics/v_temperatedesert", true);

            ResourceManager.Add<BitmapResource>("plant_dead", "Graphics/plant_dead.png", true);

            ResourceManager.Add<AudioResource>("menu", "Audio/misc_menu.wav");
            ResourceManager.Add<AudioResource>("nom", "Audio/nom.wav");
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

            ActiveCamera = new Camera(889, 4444, 500, 2500)
            {
                Bounds = new System.Drawing.RectangleF(0, 0, WorldSize, WorldSize)
            };
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
