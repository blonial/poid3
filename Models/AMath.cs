using AForge.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace poid.Models
{
    public static class AMath
    {
        #region Static Methods

        public static Complex[] FFT(double[] channel)
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

        public static List<Complex> CalculateFastTransform(List<Complex> signal, List<Complex> wCoefficients, int recursionDepth)
        {
            List<Complex> evenElements = new List<Complex>();
            List<Complex> oddElements = new List<Complex>();
            bool isEven = true;
            foreach (Complex number in signal)
            {
                if (isEven) evenElements.Add(number);
                else oddElements.Add(number);
                isEven = !isEven;
            }

            List<Complex> evenElementsTransformed = evenElements;
            List<Complex> oddElementsTransformed = oddElements;
            if (evenElements.Count > 1) evenElementsTransformed = CalculateFastTransform(evenElements, wCoefficients, recursionDepth + 1);
            if (oddElements.Count > 1) oddElementsTransformed = CalculateFastTransform(oddElements, wCoefficients, recursionDepth + 1);

            List<Complex> result = new List<Complex>();
            result.AddRange(Enumerable.Repeat(new Complex(0, 0), signal.Count));

            int halfOfSampleCount = signal.Count / 2;
            for (int i = 0; i < evenElements.Count; i++)
            {
                Complex product = Complex.Multiply(wCoefficients[(int)(i * Math.Pow(2, recursionDepth))], oddElementsTransformed[i]);
                result[i] = Complex.Add(evenElementsTransformed[i], product);
                result[i + halfOfSampleCount] = Complex.Subtract(evenElementsTransformed[i], product);
            }
            return result;
        }

        private static Complex GetWCoefficient(double upperCoefficient, double lowerCoefficient, bool isNegativeExponent)
        {
            Complex result = new Complex();
            double exponent = (2.0 * Math.PI * upperCoefficient) / lowerCoefficient;
            result.Re = Math.Cos(exponent);
            result.Im = Math.Sin(exponent);
            return result;
        }

        public static List<Complex> CalculateWCoefficients(int vectorSize, bool forReverseTransform)
        {
            List<Complex> result = new List<Complex>();
            int multiplier = forReverseTransform ? -1 : 1;
            int halfOfVectorSize = vectorSize / 2;
            for (int i = 0; i < halfOfVectorSize; i++)
            {
                result.Add(GetWCoefficient(-i * multiplier, vectorSize, true));
            }

            return result;
        }

        #endregion
    }
}
