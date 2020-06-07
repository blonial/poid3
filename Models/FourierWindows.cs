using System;

namespace poid.Models
{
    public static class FourierWindows
    {
        #region Static properties

        private static double Sigma;

        #endregion

        #region Enums

        public enum WindowType { Gauss, Hamming, Hanning, Bartlett }

        #endregion

        #region Static Methods

        public static double Gauss(int i, int N)
        {
            double factor = (i - ((N - 1.0) / 2.0)) / ((Sigma * (N - 1.0)) / 2.0);
            return Math.Pow(Math.E, -1 / 2 * (factor * factor));
        }

        public static double Hamming(int i, int N)
        {
            return 0.53836 - (0.46164 * Math.Cos((2.0 * Math.PI * i) / (N - 1.0)));
        }

        public static double Hanning(int i, int N)
        {
            return 0.5 * (1.0 - Math.Cos((2.0 * Math.PI * i) / (N - 1.0)));
        }

        public static double Bartlett(int i, int N)
        {
            return Convert.ToSingle((2.0 / (N - 1)) * (((N - 1.0) / 2.0) - Math.Abs(i - ((N - 1.0) / 2.0))));
        }

        public static double[][] Calculate(double[][] channels, WindowType windowType, double sigma)
        {
            Sigma = sigma;

            Func<int, int, double> window;
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

            double[][] result = new double[channels.Length][];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[channels[i].Length];
                for (int j = 0; j < result[i].Length; j++)
                {
                    result[i][j] = channels[i][j] * window(j, result[i].Length);
                }
            }

            return result;
        }

        public static double[][] SplitData(double[] channel, int windowLength)
        {
            double[][] result = new double[channel.Length / windowLength][];
            for (int i = 0; i < channel.Length / windowLength; i++)
            {
                result[i] = new double[windowLength];
                Array.Copy(channel, i * windowLength, result[i], 0, windowLength);
            }
            return result;
        }

        #endregion
    }
}
