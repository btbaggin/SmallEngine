using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class DiamondSquareNoise
    {
        private float[,] _grid;
        private readonly int _size;
        private readonly Random _random;
        public DiamondSquareNoise(int pSize)
        {
            _size = pSize - 1;
            _random = new Random();
        }

        public float[,] Generate(float pMin, float pMax, float pNoise)
        {
            if ((_size & (_size - 1)) != 0)
            {
                throw new Exception("Size must be 2^n + 1");
            }

            float modNoise = 0f;
            _grid = new float[_size + 1, _size + 1];
            _grid[0, 0] = _random.Range(pMin, pMax);
            _grid[_size, 0] = _random.Range(pMin, pMax);
            _grid[0, _size] = _random.Range(pMin, pMax);
            _grid[_size, _size] = _random.Range(pMin, pMax);

            for (int i = _size; i > 1; i /= 2)
            {
                modNoise = (pMax - pMin) * pNoise * ((float)i / _size);

                for (int y = 0; y < _size; y += i)
                {
                    for (int x = 0; x < _size; x += i)
                    {
                        SampleDiamond(x, y, i, modNoise);
                    }
                }

                for (int y = 0; y < _size; y += i)
                {
                    for (int x = 0; x < _size; x += i)
                    {
                        SampleSquare(x, y, i, modNoise);
                    }
                }
            }

            return _grid;
        }

        public void SampleDiamond(int x, int y, int hs, float value)
        {

            float a = GetValue(x, y);
            float b = GetValue(x + hs, y);
            float c = GetValue(x, y + hs);
            float d = GetValue(x + hs, y + hs);

            SetValue(x + (hs / 2), y + (hs / 2), (float)((a + b + c + d) / 4.0) + _random.Range(-value, value));
        }

        public void SampleSquare(int x, int y, int hs, float value)
        {
            var a = GetValue(x, y);
            var b = GetValue(x + hs, y);
            var c = GetValue(x, y + hs);
            var d = GetValue(x + hs, y + hs);
            var cn = GetValue(x + (hs / 2), y + (hs / 2));

            float d0 = (a + b + cn + GetValue(x + (hs / 2), y - (hs / 2))) / 4.0f;
            float d1 = (a + cn + c + GetValue(x - (hs / 2), y + (hs / 2))) / 4.0f;
            float d2 = (b + cn + d + GetValue(x + hs + (hs / 2), y + (hs / 2))) / 4.0f;
            float d3 = (cn + c + d + GetValue(x + (hs / 2), y + hs + (hs / 2))) / 4.0f;

            SetValue(x + (hs / 2), y, d0 + _random.Range(-value, value));
            SetValue(x, y + (hs / 2), d1 + _random.Range(-value, value));
            SetValue(x + hs, y + (hs / 2), d2 + _random.Range(-value, value));
            SetValue(x + (hs / 2), y + hs, d3 + _random.Range(-value, value));
        }

        private float GetValue(int x, int y)
        {
            return _grid[x & (_size - 1), y & (_size - 1)];
        }

        private void SetValue(int x, int y, float value)
        {
            _grid[x & (_size - 1), y & (_size - 1)] = value;
        }

        
    }
}
