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

        public ObservableCollection<WindowType> WindowTypes { get; } = new ObservableCollection<WindowType> { WindowType.Gauss, WindowType.Hamming, WindowType.Hanning, WindowType.Bartlett };

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

        private int? _AutocorrelationFreq = null;
        public int? AutocorrelationFreq
        {
            get
            {
                return _AutocorrelationFreq;
            }
            set
            {
                _AutocorrelationFreq = value;
                NotifyPropertyChanged("AutocorrelationFreq");
            }
        }

        private int? _FourierFreq = null;
        public int? FourierFreq
        {
            get
            {
                return _FourierFreq;
            }
            set
            {
                _FourierFreq = value;
                NotifyPropertyChanged("FourierFreq");
            }
        }

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
                WavReader.Read(openFileDialog.FileName, out leftChannel, out rightChannel);
                this.Channel = leftChannel;
                this.FileName = openFileDialog.FileName;
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
            float[] autocorrelation;
            this.AutocorrelationFreq = Models.Autocorrelation.CalculateFreq(this.Channel, out autocorrelation);
            List<DataPoint> signal = new List<DataPoint>();
            for (int i = 0; i < autocorrelation.Length; i++)
            {
                signal.Add(new DataPoint(i, autocorrelation[i]));
            }
            this.AutocorrelationData = signal;
        }

        private void FourierSpectrumAnalysis(object o)
        {
            try
            {
                float[][] data = FourierWindows.Calculate(FourierWindows.SplitData(this.Channel, 2205), this.SelectedWindowType, float.Parse(this.Sigma));
            }
            catch (Exception e)
            {
                Notify.Error(e.Message);
            }

        }

        #endregion
    }
}
