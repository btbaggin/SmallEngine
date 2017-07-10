using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolusim
{
    class HeightMap
    {
        private float[,] values;
        private int _width;
        private int _height;
        private int _featureSize = 10;
        private Random r;

        public HeightMap()
        {
            _width = 100;
            _height = 100;
            values = new float[_width, _height];
            r = new Random();
            Generate();

            int samplesize = _featureSize;

            float scale = 10;

            while (samplesize > 1)
            {
                DiamondSquare(samplesize, scale);

                samplesize /= 2;
                scale /= 2.0f;
            }
        }

        public void Generate()
        {
            for (int y = 0; y < _height; y += _featureSize)
            {
                for (int x = 0; x < _width; x += _featureSize)
                {
                    setSample(x, y, frand());  //IMPORTANT: frand() is a random function that returns a value between -1 and 1.
                }
            }
        }

        public void sampleSquare(int x, int y, int size, float value)
        {
            int hs = size / 2;

            // a     b 
            //
            //    x
            //
            // c     d

            float a = sample(x - hs, y - hs);
            float b = sample(x + hs, y - hs);
            float c = sample(x - hs, y + hs);
            float d = sample(x + hs, y + hs);

            setSample(x, y, (float)((a + b + c + d) / 4.0) + value);

        }

        void DiamondSquare(int stepsize, float scale)
        {

            int halfstep = stepsize / 2;

            for (int y = halfstep; y < _height + halfstep; y += stepsize)
            {
                for (int x = halfstep; x < _width + halfstep; x += stepsize)
                {
                    sampleSquare(x, y, stepsize, frand() * scale);
                }
            }

            for (int y = 0; y < _height; y += stepsize)
            {
                for (int x = 0; x < _width; x += stepsize)
                {
                    sampleDiamond(x + halfstep, y, stepsize, frand() * scale);
                    sampleDiamond(x, y + halfstep, stepsize, frand() * scale);
                }
            }
        }


        public void sampleDiamond(int x, int y, int size, float value)
        {
            int hs = size / 2;

            //   c
            //
            //a  x  b
            //
            //   d

            float a = sample(x - hs, y);
            float b = sample(x + hs, y);
            float c = sample(x, y - hs);
            float d = sample(x, y + hs);

            setSample(x, y, (float)((a + b + c + d) / 4.0) + value);
        }

        private float frand()
        {
            return (float)(r.NextDouble() * 2) - 1;
        }

        public float sample(int x, int y)
        {
            //return values[(x & (_width - 1)) + (y & (_height - 1)) * _width];
            return values[x & (_width - 1), y & (_height - 1)];
        }

        public void setSample(int x, int y, float value)
        {
            values[x & (_width - 1), y & (_height - 1)] = value;
            //values[(x & (_width - 1)) +(y & (_height - 1)) * _width] = value;
        }
    }
}
