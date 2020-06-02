namespace poid.Models
{
    static class Autocorrelation
    {
        #region Static methods

        public static int CalculateFreq(float[] channel, int sampleRate)
        {
            float[] autocorrelation = new float[channel.Length];

            for (int m = 0; m < autocorrelation.Length; m++)
            {
                float sum = 0;
                for (int n = 0; n < channel.Length - m; n++)
                {
                    sum += channel[n] * channel[n + m];
                }
                autocorrelation[m] = sum;
            }

            for (int i = 1; i < autocorrelation.Length - 1; i++)
            {
                if (autocorrelation[i - 1] < autocorrelation[i] && autocorrelation[i + 1] < autocorrelation[i])
                {
                    return sampleRate / i;
                }
            }

            return -1;
        }

        public static int[] CalculateFrequencies(float[][] channels, int sampleRate)
        {
            int[] result = new int[channels.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = CalculateFreq(channels[i], sampleRate);
            }

            return result;
        }

        public static float[] CalculateAutocorrelation(float[] channel)
        {
            float[] autocorrelation = new float[channel.Length];

            for (int m = 0; m < autocorrelation.Length; m++)
            {
                float sum = 0;
                for (int n = 0; n < channel.Length - m; n++)
                {
                    sum += channel[n] * channel[n + m];
                }
                autocorrelation[m] = sum;
            }

            return autocorrelation;
        }

        #endregion
    }
}
