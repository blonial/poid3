using System;
using System.Collections.Generic;
using System.Linq;

namespace poid.Models
{
    public class SoundHelper
    {
        public static int GetWindowSize(int sampleRate)
        {
            return sampleRate / 20;
        }
    }
}
