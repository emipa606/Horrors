﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using UnityEngine;

/*
 * Author: TurboTuTone
 * Based on Ken Perlin's Improved Noise.
 */

namespace Horrors
{
    public class NoiseLayer
    {
        public int[] perm;
        public double freq;
        public double amp;
    }

    public class TurboNoise
    {
        private int[] perm = {	151,160,137,91,90,15,131, 13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
								190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11, 32,57,177,33, 88,237,149,56,87,174,
								20,125,136,171,168, 68,175,74,165,71,134,139,48, 27,166,77,146,158,231, 83,111,229,122,60,211,133,
								230,220,105,92,41,55,46,245,40,244,102,143, 54,65,25,63,161, 1,216,80,73,209,76,132,187,208,89,18,
								169,200,196,135,130,116,188,159,86,164,100,109,198,173,186, 3,64, 52,217,226,250,124,123,5,202,38,
								147,118,126,255,82,85,212,207,206,59,227,47,16,58, 17,182,189,28,42,223,183,170,213,119,248,152,2,
								44,154,163, 70,221,153,101,155,167, 43,172,9,129,22,39,253, 19,98,108,110,79,113,224,232,178, 185,
								112,104,218,246,97,228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,
								49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127,4,150,254,138,236,205,93,222,114,
								67,29,24,72,243,141,128,195,78,66,215,61,156,180 };
        private List<NoiseLayer> layers;

        public TurboNoise()
        {
            this.layers = new List<NoiseLayer>();
        }

        private double lerp(double t, double a, double b) { return a + t * (b - a); }
        private double fade(double t) { return t * t * t * (t * (t * 6 - 15) + 10); }

        public double noise(double x, double y, int[] layer)
        {
            int X = (int)Math.Floor(x) & 255,
                Y = (int)Math.Floor(y) & 255;

            double fx = this.fade(x - Math.Floor(x)),
                   fy = this.fade(y - Math.Floor(y));

            int AA = layer[X] + Y, AB = layer[X] + Y + 1,
                BA = layer[X + 1] + Y, BB = layer[X + 1] + Y + 1;

            return this.lerp(fy, this.lerp(fx, this.perm[layer[AA]], this.perm[layer[BA]]),
                                    this.lerp(fx, this.perm[layer[AB]], this.perm[layer[BB]]));
        }

        public double layeredNoise(double x, double y)
        {
            double fn = 0.0D, maxSum = 0.0D;
            for (int i = 0; i < this.layers.Count; i++)
            {
                maxSum += this.layers[i].amp;
                fn += (this.noise(x * this.layers[i].freq, y * this.layers[i].freq, this.layers[i].perm) * this.layers[i].amp);
            }
            return (fn / maxSum) / 255.0D;
        }

        public void addLayer(int seed, double freq, double amp)
        {
            int s = (int)Math.Floor(seed - Math.Floor(seed / 256D) * 256);
            NoiseLayer l = new NoiseLayer();
            l.amp = amp;
            l.freq = freq;
            l.perm = new int[512];
            for (int i = 0; i < 256; i++)
            {
                int ii = (int)Math.Floor((seed + i) - Math.Floor((seed + i) / 256D) * 256);
                l.perm[ii] = this.perm[i];
                l.perm[255 + ii] = l.perm[ii];
            }
            this.layers.Add(l);
        }

    }
}

