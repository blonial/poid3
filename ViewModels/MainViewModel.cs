using AForge.Math;
using Microsoft.Win32;
using OxyPlot;
using poid.Commands;
using poid.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using static poid.Models.FourierWindows;

namespace poid.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedEvent
    {
        #region Properties

        #region View properties

        public ObservableCollection<WindowType> WindowTypes { get; } = new ObservableCollection<WindowType> { WindowType.Gauss, WindowType.Hamming, WindowType.Hanning, WindowType.Bartlett };

        private string _AutocorrelationResult = "";
        public string AutocorrelationResult
        {
            get
            {
                return _AutocorrelationResult;
            }
            private set
            {
                _AutocorrelationResult = value;
                NotifyPropertyChanged("AutocorrelationResult");
            }
        }

        private string _FourierResult = "";
        public string FourierResult
        {
            get
            {
                return _FourierResult;
            }
            private set
            {
                _FourierResult = value;
                NotifyPropertyChanged("FourierResult");
            }
        }

        #endregion

        #region File properties

        private double[] _Channel;
        public double[] Channel
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

        private string _Frequency = "100";
        public string Frequency
        {
            get
            {
                return _Frequency;
            }
            set
            {
                _Frequency = value;
                NotifyPropertyChanged("Frequency");
            }
        }

        private string _Duration = "2";
        public string Duration
        {
            get
            {
                return _Duration;
            }
            set
            {
                _Duration = value;
                NotifyPropertyChanged("Duration");
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
            this.InitializeEventListeners();
        }

        #endregion

        #region Initializers

        private void InitializeCommands()
        {
            this._LoadFile = new RelayCommand(this.LoadFile);
            this._SaveFile = new RelayCommand(this.SaveFile);
            this._Autocorrelation = new RelayCommand(o => this.FileName != null, this.Autocorrelation);
            this._FourierSpectrumAnalysis = new RelayCommand(o => this.FileName != null, this.FourierSpectrumAnalysis);
            this._SaveAutocorrelationAsWavFile = new RelayCommand(o => this.AutocorrelationFreq != null, this.SaveAutocorrelationAsWavFile);
            this._SaveFourierAsWavFile = new RelayCommand(o => this.FourierFreq != null, this.SaveFourierAsWavFile);
        }

        private void InitializeWindowTypes()
        {
            this.SelectedWindowType = this.WindowTypes[1];
        }

        private void InitializeEventListeners()
        {
            this.PropertyChanged += this.HandleAutocorrelationFrequencyChanged;
            this.PropertyChanged += this.HandleFourierFrequencyChanged;
        }

        #endregion

        #region Event listeners

        private void HandleAutocorrelationFrequencyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "AutocorrelationFreq")
            {
                if (this.AutocorrelationFreq != null)
                {
                    double distance = Math.Round(2048.0 * 1000 / this.SampleRate, 2, MidpointRounding.AwayFromZero);
                    double start = 0;
                    string result = "";
                    for (int i = 0; i < this.AutocorrelationFreq.Length; i++)
                    {
                        double end = start + distance;
                        result += start + "ms - " + end + "ms [" + this.AutocorrelationFreq[i] + "Hz]; ";
                        start += distance;
                    }
                    this.AutocorrelationResult = result;
                }
                else
                {
                    this.AutocorrelationResult = "";
                }
            }
        }

        private void HandleFourierFrequencyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "FourierFreq")
            {
                if (this.FourierFreq != null)
                {
                    double distance = Math.Round(2048.0 * 1000 / this.SampleRate, 2, MidpointRounding.AwayFromZero);
                    double start = 0;
                    string result = "";
                    for (int i = 0; i < this.FourierFreq.Length; i++)
                    {
                        double end = start + distance;
                        result += start + "ms - " + end + "ms [" + this.FourierFreq[i] + "Hz]; ";
                        start += distance;
                    }
                    this.FourierResult = result;
                }
                else
                {
                    this.FourierResult = "";
                }
            }
        }

        #endregion

        #region Commands

        public ICommand _LoadFile { get; private set; }

        public ICommand _SaveFile { get; private set; }

        public ICommand _Autocorrelation { get; private set; }

        public ICommand _FourierSpectrumAnalysis { get; private set; }

        public ICommand _SaveAutocorrelationAsWavFile { get; private set; }

        public ICommand _SaveFourierAsWavFile { get; private set; }

        #endregion

        #region Methods

        #region File methods

        private void LoadFile(object o)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse WAV Files";
            openFileDialog.DefaultExt = "wav";
            openFileDialog.Filter = "WAV files (*.wav)|*.wav";

            if (openFileDialog.ShowDialog() == true)
            {
                WavData data = WavReader.ReadData(openFileDialog.FileName);
                this.Channel = data.Samples;
                this.FileName = openFileDialog.FileName;
                this.SampleRate = data.FormatChunk.SampleRate;
                List<DataPoint> signal = new List<DataPoint>();
                for (int i = 0; i < this.Channel.Length; i++)
                {
                    signal.Add(new DataPoint(i, this.Channel[i]));
                }
                this.Signal = signal;
                this.AutocorrelationFreq = null;
                this.FourierFreq = null;
                this.AutocorrelationData = null;
                Notify.Info("WAV file loaded sucessfully!");
            }
        }

        private void SaveFile(object o)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save WAV File";
            saveFileDialog.DefaultExt = "wav";
            saveFileDialog.Filter = "WAV files (*.wav)|*.wav";

            try
            {
                double frequency = double.Parse(this._Frequency);
                int duration = int.Parse(this._Duration);

                if (saveFileDialog.ShowDialog() == true)
                {
                    WavWritter.Save(saveFileDialog.FileName, frequency, duration);
                    Notify.Info("Wav file saved successfully!");
                }
            }
            catch (Exception e)
            {
                Notify.Error(e.Message);
            }
        }

        private void SaveFreqArrayAsWavFile(int[] freq)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save WAV File";
            saveFileDialog.DefaultExt = "wav";
            saveFileDialog.Filter = "WAV files (*.wav)|*.wav";

            try
            {
                double frequency = double.Parse(this._Frequency);
                int duration = int.Parse(this._Duration);

                if (saveFileDialog.ShowDialog() == true)
                {
                    WavWritter.SaveFrequencies(saveFileDialog.FileName, freq);
                    Notify.Info("Wav file saved successfully!");
                }
            }
            catch (Exception e)
            {
                Notify.Error(e.Message);
            }
        }

        #endregion

        #region Operations

        private void Autocorrelation(object o)
        {
            double[] autocorrelation = Models.Autocorrelation.CalculateAutocorrelation(this.Channel);

            List<DataPoint> signal = new List<DataPoint>();
            for (int i = 0; i < autocorrelation.Length; i++)
            {
                signal.Add(new DataPoint(i, autocorrelation[i]));
            }
            this.AutocorrelationData = signal;

            double[][] data = FourierWindows.SplitData(this.Channel, 2048);
            this.AutocorrelationFreq = Models.Autocorrelation.CalculateFrequencies(data, this.SampleRate);
        }

        private void FourierSpectrumAnalysis(object o)
        {
            try
            {
                double[][] data = FourierWindows.Calculate(FourierWindows.SplitData(this.Channel, 2048), this.SelectedWindowType, float.Parse(this.Sigma));
                int[] freq = new int[data.Length];
                for (int i = 0; i < freq.Length; i++)
                {
                    double[] data2 = new double[data[i].Length];
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        if (j != 0)
                        {
                            data2[j] = data[i][j] - 0.94 * data[i][j - 1];
                        }
                        else
                        {
                            data2[j] = data[i][j];
                        }
                    }

                    List<Complex> signal = new List<Complex>();
                    for (int j = 0; j < data2.Length; j++)
                    {
                        signal.Add(new Complex(data2[j], 0));
                    }
                    List<Complex> dft = AMath.CalculateFastTransform(signal, AMath.CalculateWCoefficients(data2.Length, false), 0);
                    dft.RemoveRange(dft.Count / 2, dft.Count / 2);

                    double[] spectrum = new double[dft.Count];
                    for (int j = 0; j < spectrum.Length; j++)
                    {
                        spectrum[j] = Math.Sqrt((dft[j].Re * dft[j].Re) + (dft[j].Im * dft[j].Im));
                    }

                    List<int> max = new List<int>();
                    for (int j = 1; j < spectrum.Length - 1; j++)
                    {
                        if (spectrum[j - 1] < spectrum[j] && spectrum[j + 1] < spectrum[j])
                        {
                            max.Add(j);
                        }
                    }

                    int globalMax = 0;
                    double globalMaxVal = 0;
                    for (int j = 0; j < max.Count; j++)
                    {
                        int index = max[j];
                        double value = spectrum[index];
                        if (value > globalMaxVal)
                        {
                            globalMax = index;
                            globalMaxVal = value;
                        }
                    }

                    double border = 0.2 * globalMaxVal;
                    List<int> bordered = new List<int>();
                    for (int j = 0; j < max.Count; j++)
                    {
                        if (spectrum[max[j]] >= border)
                        {
                            bordered.Add(max[j]);
                        }
                    }

                    double median;
                    if (bordered.Count > 1)
                    {
                        List<double> differences = new List<double>();
                        for (int j = 0; j < max.Count - 1; j++)
                        {
                            for (int k = j + 1; k < max.Count; k++)
                            {
                                differences.Add(Math.Abs(max[j] - max[k]));
                            }
                        }
                        differences.Sort();

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
                    }
                    else
                    {
                        median = bordered[0];
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

        private void SaveAutocorrelationAsWavFile(object o)
        {
            this.SaveFreqArrayAsWavFile(this.AutocorrelationFreq);
        }

        private void SaveFourierAsWavFile(object o)
        {
            this.SaveFreqArrayAsWavFile(this.FourierFreq);
        }

        #endregion

        #endregion
    }
}
