﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace Evolusim
{
    class HeightMap
    {
        float[,] _map;
        float _size;
        public HeightMap(bool pPerlin, int pSize)
        {
            _size = pSize;
            if(pPerlin)
            {
                var n = new PerlinNoise(pSize);
                _map = n.Generate(4, .7f, .4f, .3f, .1f);
            }
            else
            {
                var n = new DiamondSquareNoise(pSize);
                _map = n.Generate(-10, 10, .15f);

                float maxHeight = 0;
                float maxDepth = 1;
                for (int x = 0; x < pSize; x++)
                {
                    for (int y = 0; y < pSize; y++)
                    {
                        if (_map[x, y] > maxHeight) maxHeight = _map[x, y];
                        if (_map[x, y] < maxDepth) maxDepth = _map[x, y];
                    }
                }

                //Scale all points so they are within max/min
                maxDepth = Math.Abs(maxDepth);
                for (int x = 0; x < pSize; x++)
                {
                    for (int y = 0; y < pSize; y++)
                    {
                        if(_map[x,y] < 0)
                        {
                            _map[x, y] = _map[x, y] / maxDepth;
                        }
                        else if(_map[x, y] > 0)
                        {
                            _map[x, y] = _map[x, y] / maxHeight;
                        }
                    }
                }
            }

            //Find max so we know how to scale
            
            
            
        }

        //TODO brush size?
        public void Raise(int pX, int pY, float pAmount)
        {
            System.Diagnostics.Debug.Assert(pAmount > 0);
            _map[pX, pY] += pAmount;
            if (_map[pX, pY] > 1) _map[pX, pY] = 1;
        }

        public void Lower(int pX, int pY, float pAmount)
        {
            System.Diagnostics.Debug.Assert(pAmount < 0);
            _map[pX, pY] += pAmount;
            if (_map[pX, pY] < -1) _map[pX, pY] = -1;
        }

        public float Query(int pX, int pY)
        {
            return _map[pX, pY];
        }
    }
}
