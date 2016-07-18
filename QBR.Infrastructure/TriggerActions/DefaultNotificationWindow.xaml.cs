// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Windows;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace QBR.Infrastructure.TriggerActions
{
    /// <summary>
    /// Interaction logic for NotificationChildWindow.xaml
    /// </summary>
    public partial class DefaultNotificationWindow : Window
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultNotificationWindow"/>
        /// </summary>
        public DefaultNotificationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets or gets the <see cref="INotification"/> shown by this window./>
        /// </summary>
        public Notification Notification 
        {
            get
            {
                return this.DataContext as Notification;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
