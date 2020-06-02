using AForge.Math;
using Microsoft.Win32;
using OxyPlot;
using poid.Commands;
using poid.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static poid.Models.FourierWindows;

namespace poid.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedEvent
    {
        #region Properties

        #region View properties

        public ObservableCollection<WindowType> WindowTypes { get; } = new ObservableCollection<WindowType> { WindowType.Gauss, WindowType.Hamming, WindowType.Hanning, WindowType.Bartlett };

        #endregion

        #region File properties

        private float[] _Channel;
        public float[] Channel
        {
            get
            {
                return _Channel;
            }
            private set
            {
                _Channel = value;
                NotifyPropertyChanged("Channel");
            }
        }

        private string _FileName;
        public string FileName
        {
            get
            {
                return _FileName;
            }
            private set
            {
                _FileName = value;
                NotifyPropertyChanged("FileName");
            }
        }

        private List<DataPoint> _Signal;
        public List<DataPoint> Signal
        {
            get
            {
                return _Signal;
            }
            private set
            {
                _Signal = value;
                NotifyPropertyChanged("Signal");
            }
        }

        private int _SampleRate;
        public int SampleRate
        {
            get
            {
                return _SampleRate;
            }
            private set
            {
                _SampleRate = value;
                NotifyPropertyChanged("SampleRate");
            }
        }

        #endregion

        #region Autocorrelation properties

        private List<DataPoint> _AutocorrelationData;
        public List<DataPoint> AutocorrelationData
        {
            get
            {
                return _AutocorrelationData;
            }
            set
            {
                _AutocorrelationData = value;
                NotifyPropertyChanged("AutocorrelationData");
            }
        }

        private int[] _AutocorrelationFreq;
        public int[] AutocorrelationFreq
        {
            get
            {
                return _AutocorrelationFreq;
            }
            private set
            {
                _AutocorrelationFreq = value;
                NotifyPropertyChanged("AutocorrelationFreq");
            }
        }

        #endregion

        #region Fourier properties

        private WindowType _SelectedWindowType;
        public WindowType SelectedWindowType
        {
            get
            {
                return _SelectedWindowType;
            }
            set
            {
                _SelectedWindowType = value;
                NotifyPropertyChanged("SelectedWindowType");
            }
        }

        private string _Sigma = "0.4";
        public string Sigma
        {
            get
            {
                return _Sigma;
            }
            set
            {
                _Sigma = value;
                NotifyPropertyChanged("Sigma");
            }
        }

        private int[] _FourierFreq;
        public int[] FourierFreq
        {
            get
            {
                return _FourierFreq;
            }
            private set
            {
                _FourierFreq = value;
                NotifyPropertyChanged("FourierFreq");
            }
        }

        #endregion

        #endregion

        #region Constuctors

        public MainViewModel()
        {
            this.InitializeCommands();
            this.InitializeWindowTypes();
        }

        #endregion

        #region Initializers

        private void InitializeCommands()
        {
            this._LoadFile = new RelayCommand(this.LoadFile);
            this._Autocorrelation = new RelayCommand(o => this.FileName != null, this.Autocorrelation);
            this._FourierSpectrumAnalysis = new RelayCommand(o => this.FileName != null, this.FourierSpectrumAnalysis);
        }

        private void InitializeWindowTypes()
        {
            this.SelectedWindowType = this.WindowTypes[0];
        }

        #endregion

        #region Commands

        public ICommand _LoadFile { get; private set; }

        public ICommand _Autocorrelation { get; private set; }

        public ICommand _FourierSpectrumAnalysis { get; private set; }

        #endregion

        #region Methods

        private void LoadFile(object o)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse WAV Files";
            openFileDialog.DefaultExt = "wav";
            openFileDialog.Filter = "WAV files (*.wav)|*.wav";

            if (openFileDialog.ShowDialog() == true)
            {
                float[] leftChannel;
                float[] rightChannel;
                int sampleRate;
                WavReader.Read(openFileDialog.FileName, out leftChannel, out rightChannel, out sampleRate);
                this.Channel = leftChannel;
                this.FileName = openFileDialog.FileName;
                this.SampleRate = sampleRate;
                List<DataPoint> signal = new List<DataPoint>();
                for (int i = 0; i < this.Channel.Length; i++)
                {
                    signal.Add(new DataPoint(i, this.Channel[i]));
                }
                this.Signal = signal;
                Notify.Info("WAV file loaded sucessfully!");
            }
        }

        private void Autocorrelation(object o)
        {
            float[] autocorrelation = Models.Autocorrelation.CalculateAutocorrelation(this.Channel);

            List<DataPoint> signal = new List<DataPoint>();
            for (int i = 0; i < autocorrelation.Length; i++)
            {
                signal.Add(new DataPoint(i, autocorrelation[i]));
            }
            this.AutocorrelationData = signal;

            float[][] data = FourierWindows.SplitData(this.Channel, 2205);
            this.AutocorrelationFreq = Models.Autocorrelation.CalculateFrequencies(data, this.SampleRate);
        }

        private void FourierSpectrumAnalysis(object o)
        {
            try
            {
                float[][] data = FourierWindows.Calculate(FourierWindows.SplitData(this.Channel, 2205), this.SelectedWindowType, float.Parse(this.Sigma));
                int[] freq = new int[data.Length];
                for (int i = 0; i < 1; i++)
                {
                    Complex[] dft = AMath.FFT(data[i]);

                    double[] spectrum = new double[data[i].Length];
                    for (int j = 0; j < spectrum.Length; j++)
                    {
                        spectrum[j] = Math.Sqrt(dft[j].Re * dft[j].Re + dft[j].Im * dft[j].Im);
                    }

                    double globalMax = 0;
                    List<double> max = new List<double>();
                    for (int j = 1; j < spectrum.Length - 1; j++)
                    {
                        if (spectrum[j - 1] < spectrum[j] && spectrum[j + 1] < spectrum[j])
                        {
                            max.Add(spectrum[j]);
                            globalMax = spectrum[j] > globalMax ? spectrum[j] : globalMax;
                        }
                    }

                    List<double> bordered = new List<double>();
                    double border = 0.2 * globalMax;
                    for (int j = 0; j < max.Count; j++)
                    {
                        if (max[j] >= border)
                        {
                            bordered.Add(max[j]);
                        }
                    }
                    bordered.Sort();
                    bordered.Reverse();

                    List<double> differences = new List<double>();
                    for (int j = 0; j < bordered.Count - 1; j++)
                    {
                        for (int k = j + 1; k < bordered.Count; k++)
                        {
                            differences.Add(bordered[j] - bordered[k]);
                        }
                    }
                    differences.Sort();

                    double median = 0;
                    if (differences.Count != 1)
                    {
                        if (differences.Count % 2 == 0)
                        {
                            median = differences[differences.Count / 2];
                        }
                        else
                        {
                            int half = (differences.Count - 1) / 2;
                            median = (differences[half] + differences[half + 1]) / 2;
                        }
                    }
                    else
                    {
                        median = differences[0];
                    }

                    freq[i] = Convert.ToInt32((this.SampleRate / data[i].Length) * median);
                }
                this.FourierFreq = freq;
            }
            catch (Exception e)
            {
                Notify.Error(e.Message);
            }
        }

        #endregion
    }
}
