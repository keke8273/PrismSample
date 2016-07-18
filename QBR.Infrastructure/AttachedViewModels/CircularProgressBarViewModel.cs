using System;
using System.Windows.Controls;

namespace QBR.Infrastructure.AttachedViewModels
{
    /// <summary>
    /// An attached view model that adapts a ProgressBar control to provide properties
    /// that assist in the creation of a circular template
    /// </summary>
    public class CircularProgressBarViewModel : AttachedViewModel<ProgressBar>
    {
        #region fields

        private double _angle;

        private double _centreX;

        private double _centreY;

        private double _radius;

        private double _innerRadius;

        private double _diameter;

        private double _percent;

        private double _holeSizeFactor = 0.0;

        protected ProgressBar _progressBar;

        private PropertyChangeNotifier _valueNotifier;

        private PropertyChangeNotifier _maximumNotifier;

        private PropertyChangeNotifier _minimumNotifier;

        #endregion

        #region properties

        public double Percent
        {
            get { return _percent; }
            set { _percent = value; OnPropertyChanged("Percent"); }
        }

        public double Diameter
        {
            get { return _diameter; }
            set { _diameter = value; OnPropertyChanged("Diameter"); }
        }

        public double Radius
        {
            get { return _radius; }
            set { _radius = value; OnPropertyChanged("Radius"); }
        }

        public double InnerRadius
        {
            get { return _innerRadius; }
            set { _innerRadius = value; OnPropertyChanged("InnerRadius"); }
        }

        public double CentreX
        {
            get { return _centreX; }
            set { _centreX = value; OnPropertyChanged("CentreX"); }
        }

        public double CentreY
        {
            get { return _centreY; }
            set { _centreY = value; OnPropertyChanged("CentreY"); }
        }

        public double Angle
        {
            get { return _angle; }
            set { _angle = value; OnPropertyChanged("Angle"); }
        }

        public double HoleSizeFactor
        {
            get { return _holeSizeFactor; }
            set { _holeSizeFactor = value; ComputeViewModelProperties(); }
        }

        #endregion

        protected override void SetTemplatedElement(ProgressBar templatedElement)
        {
            _progressBar = templatedElement;
            _progressBar.SizeChanged += (s, e) => ComputeViewModelProperties();

            _valueNotifier = new PropertyChangeNotifier(_progressBar, "Value");
            _valueNotifier.ValueChanged += (s,e) => ComputeViewModelProperties();

            _maximumNotifier = new PropertyChangeNotifier(_progressBar, "Maximum");
            _maximumNotifier.ValueChanged += (s, e) => ComputeViewModelProperties();

            _minimumNotifier = new PropertyChangeNotifier(_progressBar, "Minimum");
            _minimumNotifier.ValueChanged += (s, e) => ComputeViewModelProperties();

            ComputeViewModelProperties();
        }

        /// <summary>
        /// Re-computes the various properties that the elements in the template bind to.
        /// </summary>
        protected override void ComputeViewModelProperties()
        {
            if (_progressBar == null)
                return;

            Angle = (_progressBar.Value - _progressBar.Minimum) * 360 / (_progressBar.Maximum - _progressBar.Minimum);
            CentreX = _progressBar.ActualWidth / 2;
            CentreY = _progressBar.ActualHeight / 2;
            Radius = Math.Min(CentreX, CentreY);
            Diameter = Radius * 2;
            InnerRadius = Radius * HoleSizeFactor;
            Percent = Angle / 360;
        }
    }
}
