using System;

namespace poid.Models
{
    public static class FourierWindows
    {
        #region Static properties

        private static float Sigma;

        #endregion

        #region Enums

        public enum WindowType { Gauss, Hamming, Hanning, Bartlett }

        #endregion

        #region Static Methods

        public static float Gauss(int i, int N)
        {
            float factor = Convert.ToSingle((i - ((N - 1.0) / 2.0)) / ((Sigma * (N - 1.0)) / 2.0));
            return Convert.ToSingle(Math.Pow(Math.E, -1 / 2 * (factor * factor)));
        }

        public static float Hamming(int i, int N)
        {
            return Convert.ToSingle(0.53836 - (0.46164 * Math.Cos((2.0 * Math.PI * i) / (N - 1.0))));
        }

        public static float Hanning(int i, int N)
        {
            return Convert.ToSingle(0.5 * (1.0 - Math.Cos((2.0 * Math.PI * i) / (N - 1.0))));
        }

        public static float Bartlett(int i, int N)
        {
            return Convert.ToSingle((2.0 / (N - 1)) * (((N - 1.0) / 2.0) - Math.Abs(i - ((N - 1.0) / 2.0))));
        }

        public static float[][] Calculate(float[][] channels, WindowType windowType, float sigma)
        {
            Sigma = sigma;

            Func<int, int, float> window = Gauss;
            switch (windowType)
            {
                default:
                case WindowType.Gauss:
                    window = Gauss;
                    break;
                case WindowType.Hamming:
                    window = Hamming;
                    break;
                case WindowType.Hanning:
                    window = Hanning;
                    break;
                case WindowType.Bartlett:
                    window = Bartlett;
                    break;
            }

            float[][] result = new float[channels.Length][];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new float[channels[i].Length];
                for (int j = 0; j < result[i].Length; j++)
                {
                    result[i][j] = channels[i][j] * window(j, result[i].Length);
                }
            }

            return result;
        }

        public static float[][] SplitData(float[] channel, int windowLength)
        {
            float[][] result = new float[channel.Length / windowLength][];
            for (int i = 0; i < channel.Length / windowLength; i++)
            {
                result[i] = new float[windowLength];
                Array.Copy(channel, i * windowLength, result[i], 0, windowLength);
            }
            return result;
        }

        #endregion
    }
}
