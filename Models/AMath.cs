using AForge.Math;
using System;

namespace poid.Models
{
    public static class AMath
    {
        #region Static Methods

        public static Complex[] FFT(float[] channel)
        {
            Complex[] data = new Complex[channel.Length];

            for (int i = 0; i < channel.Length; i++)
            {
                data[i] = new Complex(channel[i], 0);
            }

            FourierTransform.FFT(data, FourierTransform.Direction.Forward);

            return data;
        }

        public static float[] IFFT(Complex[] data)
        {
            FourierTransform.FFT(data, FourierTransform.Direction.Backward);

            float[] result = new float[data.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToSingle(-data[i].Re);
            }

            return result;
        }

        #endregion
    }
}
