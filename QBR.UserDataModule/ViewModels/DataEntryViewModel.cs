using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Utilities;

namespace QBR.UserDataModule.ViewModels
{
    public class DataEntryViewModel : BindableObject
    {
        private readonly IUserEntryService _userEntryService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        //private readonly IRegionManager _regionManager;
        private bool _isReadOnly;

        public DataEntryViewModel(IUserEntryService userEntryService, IEventAggregator eventAggregator,
            IDialogService dialogService, IRegionManager regionManager)
        {
            _userEntryService = userEntryService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TestStartedEvent>().Subscribe(OnTestStarted);
            _eventAggregator.GetEvent<TestCompletedEvent>().Subscribe(OnTestCompleted);
            _dialogService = dialogService;
            //_regionManager = regionManager;

            //StartTestCommand = new DelegateCommand(StartTest, CanStartTest)
            //    .ListenOn(this, self => self.HasError)
            //    .ListenOn(this, self => self.OperatorID)
            //    .ListenOn(this, self => self.BankID)
            //    .ListenOn(this, self => self.TestID)
            //    .ListenOn(this, self => self.OutputDirectory)
            //    .ListenOn(this, self => self.BatchNumber)
            //    .ListenOn(this, self => self.Project)
            //    .ListenOn(this, self => self.TestTarget)
            //    .ListenOn(this, self => self.TargetHCT)
            //    .ListenOn(this, self => self.TargetINR);

            OpenFolderBrowserCommand = new DelegateCommand(OpenFolderBrower);
        }

        #region IDataErrorInfo Members
        //public string this[string columnName]
        //{
        //    get
        //    {
        //        var errorMessage = string.Empty;
        //        switch (columnName)
        //        {
        //            case "OperatorID":
        //                if (string.IsNullOrEmpty(OperatorID))
        //                {
        //                    errorMessage = "Operator ID can not empty";
        //                }
        //                break;
        //        }
        //        return errorMessage;
        //    }
        //}

        //public string Error { get; private set; } 
        #endregion

        #region Properties
        public string OperatorID
        {
            get { return _userEntryService.OperatorID; }
            set
            {
                _userEntryService.OperatorID = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, string>(self => self.OperatorID));
            }
        }

        public int BankID
        {
            get { return _userEntryService.BankID; }
            set
            {
                _userEntryService.BankID = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, int>(self => self.BankID));
            }
        }

        public int TestID
        {
            get { return _userEntryService.TestID; }
            set
            {
                _userEntryService.TestID = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, int>(self => self.TestID));                
            }
        }

        public string OutputDirectory
        {
            get { return _userEntryService.OutputDirectory; }
            set
            {
                _userEntryService.OutputDirectory = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, string>(self => self.OutputDirectory));                
            }
        }

        public string BatchNumber
        {
            get { return _userEntryService.BatchNumber; }
            set
            {
                _userEntryService.BatchNumber = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, string>(self => self.BatchNumber));                
            }
        }

        public bool HasError
        {
            get { return _userEntryService.HasError; }
            set
            {
                _userEntryService.HasError = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, bool>(self => self.HasError));                
            }
        }

        public StripType StripType
        {
            get { return _userEntryService.StripType; }
            set
            {
                _userEntryService.StripType = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, StripType>(self => self.StripType));
            }
        }

        public string TestTarget
        {
            get { return _userEntryService.TestTarget; }
            set
            {
                _userEntryService.TestTarget = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, string>(self => self.TestTarget));
                UpdateTestTarget();
            }
        }

        public double TargetINR
        {
            get { return _userEntryService.TargetINR; }
            set
            {
                _userEntryService.TargetINR = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, double>(self => self.TargetINR));
            }
        }

        public double TargetHCT
        {
            get { return _userEntryService.TargetHCT; }
            set
            {
                _userEntryService.TargetHCT = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, double>(self => self.TargetHCT));
            }
        }

        public string Comments
        {
            get { return _userEntryService.Comments; }
            set
            {
                _userEntryService.Comments = value;
                OnPropertyChanged(PropertyHelpers.GetPropertyName<DataEntryViewModel, string>(self => self.Comments));
            }
        }

        public StringCollection RecentOutputDirectories
        {
            get { return _userEntryService.RecentOutputDirectories; }
        }

        public IEnumerable<string> StripTypeDescriptions
        {
            get { return EnumExtensions.GetAllDescriptions<StripType>(); }
        }

        public ICommand OpenFolderBrowserCommand { get; private set; }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                SetProperty(ref _isReadOnly, value, "IsReadOnly");
            }
        }
        #endregion

        #region Functions

        private void OpenFolderBrower()
        {
            var selectedPath = _dialogService.ShowFolderBrowserDialog();

            if (selectedPath != string.Empty)
            {
                OutputDirectory = selectedPath;
            }
        }

        private void UpdateTestTarget()
        {
            if (StripType == StripType.Proteus)
            {
                TargetHCT = 0;
                if (TestTarget == "MultiCalibrator lvl1")
                {
                    TargetINR = 1.0;
                }
                else if (TestTarget == "MultiCalibrator lvl2")
                {
                    TargetINR = 2.0;                    
                }
                else if (TestTarget == "MultiCalibrator lvl3")
                {
                    TargetINR = 3.0;
                }
                else if (TestTarget == "MultiCalibrator lvl4")
                {
                    TargetINR = 4.0;
                }
                else if (TestTarget == "MultiCalibrator lvl5")
                {
                    TargetINR = 5.0;
                }
                else if (TestTarget == "MultiCalibrator lvl6")
                {
                    TargetINR = 6.0;
                }
            }
        }

        private void OnTestCompleted(object obj)
        {
            TestID++;
            IsReadOnly = false;
        }

        private void OnTestStarted(object obj)
        {
            IsReadOnly = true;
        }
        #endregion
    }
}
