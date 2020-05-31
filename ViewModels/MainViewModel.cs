using Microsoft.Win32;
using poid.Commands;
using poid.Models;
using System.Windows.Input;

namespace poid.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedEvent
    {
        #region Properties

        private float[] _R;
        public float[] R
        {
            get
            {
                return _R;
            }
            private set
            {
                _R = value;
                NotifyPropertyChanged("R");
            }
        }

        private float[] _L;
        public float[] L
        {
            get
            {
                return _L;
            }
            private set
            {
                _L = value;
                NotifyPropertyChanged("L");
            }
        }

        private MonoType? _FileMonoType;
        public MonoType? FileMonoType
        {
            get
            {
                return _FileMonoType;
            }
            private set
            {
                _FileMonoType = value;
                NotifyPropertyChanged("FileMonoType");
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

        #endregion

        #region Enums

        public enum MonoType { Mono, Stereo }

        #endregion

        #region Constuctors

        public MainViewModel()
        {
            this.InitializeCommands();
        }

        #endregion

        #region Initializers

        private void InitializeCommands()
        {
            this._LoadFile = new RelayCommand(this.LoadFile);
        }

        #endregion

        #region Commands

        public ICommand _LoadFile { get; private set; }

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
                float[] r;
                float[] l;
                if (WavReader.Read(openFileDialog.FileName, out l, out r))
                {
                    this.R = r;
                    this.L = l;
                    this.FileMonoType = r == null ? MonoType.Mono : MonoType.Stereo;
                    this.FileName = openFileDialog.FileName;
                    Notify.Info("Image loaded successfully!");
                }
                else
                {
                    Notify.Info("Failed to load file!");
                    this.R = null;
                    this.L = null;
                    this.FileMonoType = null;
                    this.FileName = null;
                }
            }
        }

        #endregion
    }
}
