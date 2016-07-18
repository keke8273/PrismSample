// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Windows;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace QBR.Infrastructure.TriggerActions
{
    /// <summary>
    /// Interaction logic for ConfirmationChildWindow.xaml
    /// </summary>
    public partial class DefaultConfirmationWindow : Window
    {
        /// <summary>
        /// Creates a new instance of ConfirmationChildWindow.
        /// </summary>
        public DefaultConfirmationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets or gets the <see cref="IConfirmation"/> shown by this window./>
        /// </summary>
        public Confirmation Confirmation
        {
            get
            {
                return this.DataContext as Confirmation;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmation.Confirmed = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmation.Confirmed = false;
            this.Close();
        }
    }
}
