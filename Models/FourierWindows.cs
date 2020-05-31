using System;

namespace poid.Models
{
    public static class FourierWindows
    {
        #region Static Methods

        public static float Gauss(int i, int N, float sigma)
        {
            float factor = Convert.ToSingle((i - ((N - 1.0) / 2.0)) / ((sigma * (N - 1.0)) / 2.0));
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

        #endregion
    }
}
