﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Input;
using SmallEngine.Audio;
using SmallEngine.Graphics;

namespace SmallEngineTest
{
    class TestGame : Game
    {
        private Aquarium _aquarium;
        private AudioResource _bubbles;
        public override void Initialize()
        {
            //Form.FullScreen = true;
            PlayInBackground = false;
            Form.Width = 640;
            Form.Height = 480;

            InputManager.AddMapping("createfish", Keys.Space, 1000);
            InputManager.AddMapping("feed", Mouse.Left, 50);
            InputManager.AddMapping("exit", Keys.Escape);
            _currentState = InputManager.GetInputState();
            _previousState = InputManager.GetInputState();

            SceneManager.BeginScene("fish1");
            _aquarium = new Aquarium(this) { Persistant = true };
            SceneManager.AddToScene(_aquarium);
            //SceneManager.AddToScene(_swarm);
        }

        public override void SceneBegin(string pScene)
        {
            _bubbles.Play(.1f);
            switch(pScene)
            {
                case "fish1":
                    break;
            }
        }

        public override void SceneEnd(string pScene)
        {
            _bubbles.Stop();
            switch(pScene)
            {
                case "fish1":
                    break;
            }
        }

        public override void LoadContent()
        {
            ResourceManager.Add<BitmapResource>("aquarium_background", "Graphics/aquarium_background.jpg");
            ResourceManager.Add<BitmapResource>("fish", "Graphics/fish.png");
            ResourceManager.Add<BitmapResource>("fish_hungry", "Graphics/fish_hungry.gif");
            ResourceManager.Add<BitmapResource>("food", "Graphics/food.png");
            ResourceManager.Add<AudioResource>("bubble_large", "Audio/bubble_large.wav");
            ResourceManager.Add<AudioResource>("bubble", "Audio/bubble.wav");
            _bubbles = ResourceManager.Add<AudioResource>("bubble_backgroud", "Audio/bubble_background.wav");
            ResourceManager.AddGroup("bubbles", new string[]{ "bubble_large", "bubble"});
            Graphics.DefineColor(System.Drawing.Color.White);
            Graphics.DefineColor(System.Drawing.Color.Blue);
        }

        InputState _previousState;
        InputState _currentState;
        public override void Update(float pDeltaTime)
        {
            _currentState = InputManager.GetInputState();
            _currentState = InputManager.GetInputState();
            if (_currentState.IsPressed("createfish") && !_previousState.IsPressed("createfish"))
            {
                var f = new Fish(_aquarium);
                _aquarium.AddFish(f);
                SceneManager.AddToScene(f);
            }

            if (_currentState.IsPressed("feed") && !_previousState.IsPressed("feed"))
            {
                var f = new Food();
                _aquarium.AddFood(f);
                SceneManager.AddToScene(f);
            }

            if (_currentState.IsPressed("exit"))
            {
                Exit();
            }

            Form.Text = CurrentFps.ToString();
            _previousState = _currentState;
            base.Update(pDeltaTime);
        }
    }
}
