using UnityEngine;

namespace Roguelike.Noise
{
    public enum MapType
    {
        PERLIN_NOISE,
        DIAMOND_SQUARE,
        PERLIN_FRACTAL_NOISE,
    };

    public static class Noise
    {
        public static float PerlinNoise(Vector2 v)
        {
            return Mathf.PerlinNoise(v.x, v.y);
        }
    };

    public static class MapGenerator
    {
        private static void GeneratePerlinNoise(Vector2Int size, Vector2 offset, ref float[] noisemap)
        {
            for (int y = 0; y < size.y; ++y)
            {
                for (int x = 0; x < size.x; ++x)
                {
                    float dx = offset.x + (float)x / (float)size.x;
                    float dy = offset.y + (float)y / (float)size.y;
                    noisemap[y * size.x + x] = Noise.PerlinNoise(new Vector2(dx, dy));
                }
            }
            
        }

        private static void GenerateDiamondSquare(Vector2Int size, float roughness, ref float[] noisemap)
        {
            // size.x et y == 17
            noisemap[0] = Random.Range(1, 8);
            noisemap[size.x - 1] = Random.Range(1, 8);
            noisemap[(size.x - 1) * size.x] = Random.Range(1, 8);
            noisemap[size.x * size.x - 1] = Random.Range(1, 8);


            int step = size.x - 1;
            float range = roughness;

            while (step > 1)
            {
                int half = step / 2;

                // diamand
                for (int y = half; y < size.x; y += step)
                {
                    for (int x = half; x < size.x; x += step)
                    {
                        float avg = (noisemap[(y - half) * size.x + (x - half)] +
                                     noisemap[(y - half) * size.x + (x + half)] +
                                     noisemap[(y + half) * size.x + (x - half)] +
                                     noisemap[(y + half) * size.x + (x + half)]) / 4f;

                        noisemap[y * size.x + x] = avg + Random.Range(-range, range);
                    }
                }

                // square
                for (int y = 0; y < size.x; y += half)
                {
                    for (int x = (y + half) % step; x < size.x; x += step)
                    {
                        float sum = 0;
                        int count = 0;

                        if (x >= half)
                        {
                            sum += noisemap[y * size.x + (x - half)];
                            count++;
                        }
                        if (x + half < size.x)
                        {
                            sum += noisemap[y * size.x + (x + half)];
                            count++;
                        }
                        if (y >= half)
                        {
                            sum += noisemap[(y - half) * size.x + x];
                            count++;
                        }
                        if (y + half < size.x)
                        {
                            sum += noisemap[(y + half) * size.x + x];
                            count++;
                        }
                        noisemap[y * size.x + x] = Mathf.Round(sum / count + Random.Range(-1, 1) * range);
                    }
                }

                step /= 2;
                range *= 0.5f;
            }

        }

        private static void GeneratePerlinFractalNoise(Vector2Int size, Vector2 offset, ref float[] noisemap)
        {

            float frequency = 0.02f;
            int octaves = 6;
            float lacunarity = 2.0f;
            float persistance = 0.6f;

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    float total = 0;
                    float t_frequency = frequency;
                    float t_amplitude = 1.0f;
                    float maxAmplitude = 0.0f;

                    for (int k = 0; k < octaves; k++)
                    {
                        float sample_x = (x + offset.x * size.x) * t_frequency;
                        float sample_y = (y + offset.y * size.y) * t_frequency;

                        total += Noise.PerlinNoise(new Vector2(sample_x, sample_y)) * t_amplitude;

                        maxAmplitude += t_amplitude;
                        t_frequency *= lacunarity;
                        t_amplitude *= persistance;
                    }
                    noisemap[y * size.x + x] = total / maxAmplitude;
                }
            }

        }

        public static float[] Generate(Vector2Int size, Vector2 offset, MapType type)
        {
            float[] noisemap = new float[size.x * size.y];
            switch (type)
            {
                case MapType.PERLIN_NOISE:
                    GeneratePerlinNoise(size, offset, ref noisemap);
                    break;
                case MapType.DIAMOND_SQUARE:
                    GenerateDiamondSquare(size, 0.8f, ref noisemap);
                    break;
                case MapType.PERLIN_FRACTAL_NOISE:
                    GeneratePerlinFractalNoise(size, offset, ref noisemap);
                    break;
                default:
                    break;
            }
            return noisemap;
        }
    };
}