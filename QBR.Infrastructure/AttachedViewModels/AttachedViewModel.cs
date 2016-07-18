using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using QBR.Infrastructure.Utilities;

namespace QBR.Infrastructure.AttachedViewModels
{
    public abstract class AttachedViewModel<T> : FrameworkElement, INotifyPropertyChanged where T : FrameworkElement
    {
        #region INotifPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        } 
        #endregion

        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
            "Attach",
            typeof(object),
            typeof(AttachedViewModel<T>),
            new PropertyMetadata(null, OnAttachChanged));

        public static AttachedViewModel<T> GetAttach(DependencyObject d)
        {
            return (AttachedViewModel<T>)d.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject d, AttachedViewModel<T> value)
        {
            d.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// Change handler for the Attach property
        /// </summary>
        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // the target is the root element of the control template.
            // set the view model as the DataContext for the rest of the template
            FrameworkElement targetElement = d as FrameworkElement;
            AttachedViewModel<T> attachedModel = e.NewValue as AttachedViewModel<T>;
            targetElement.DataContext = attachedModel;

            // handle the loaded event
            targetElement.Loaded += Element_Loaded;
        }

        /// <summary>
        /// Handle the Loaded event of the element to which this view model is attached
        /// in order to enable the attached
        /// view model to bind to properties of the parent element
        /// </summary>
        static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            //targetElement is the root element of the control template
            FrameworkElement targetElement = sender as FrameworkElement;
            FrameworkElement parent = targetElement.Parent as FrameworkElement;

            AttachedViewModel<T> attachedModel = GetAttach(targetElement);
            attachedModel.AttachedElement = targetElement;

            var templatedElement = targetElement.Ancestors<T>().Single() as T;
            attachedModel.SetTemplatedElement(templatedElement);

            // bind the DataContext of the view model to the DataContext of the parent.
            attachedModel.SetBinding(DataContextProperty,
              new Binding("DataContext")
              {
                  Source = parent
              });
        }

        private FrameworkElement _element;

        /// <summary>
        /// The element to which this view model is attached
        /// </summary>
        public FrameworkElement AttachedElement
        {
            get { return _element; }
            set
            {
                _element = value;
                _element.SizeChanged += new SizeChangedEventHandler(AttachedElement_SizeChanged);
            }
        }

        /// <summary>
        /// Handle SizeChanged events from the element that this view model is attached to
        /// so that we can inform elements
        /// of changes in the ActualHeight / ActualWidth
        /// </summary>
        protected virtual void AttachedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnPropertyChanged("ElementHeight");
            OnPropertyChanged("ElementWidth");
        }

        public double ElementWidth
        {
            get { return _element.ActualWidth; }
        }

        public double ElementHeight
        {
            get { return _element.ActualHeight; }
        }

        protected abstract void SetTemplatedElement(T templatedElement);

        protected abstract void ComputeViewModelProperties();
    }
}
