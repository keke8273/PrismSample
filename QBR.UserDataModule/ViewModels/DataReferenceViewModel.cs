using System;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models;

namespace QBR.UserDataModule.ViewModels
{
    public class DataReferenceViewModel : BindableObject
    {
        private readonly IUserEntryService _userEntryService;
        private readonly IRegionManager _regionManager;
        private int _testId;

        public DataReferenceViewModel(IUserEntryService userEntryService, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _userEntryService = userEntryService;
            _testId = _userEntryService.TestID;
            
            _regionManager = regionManager;

            eventAggregator.GetEvent<TestCompletedEvent>().Subscribe(OnTestCompleted, ThreadOption.UIThread);
            //eventAggregator.GetEvent<TestContinuedEvent>().Subscribe(OnTestContinuedEvent);
        }

        #region Properties
        public string OperatorID
        {
            get { return _userEntryService.OperatorID; }
        }

        public int TestID
        {
            get { return _testId; }
            set
            {
                SetProperty(ref _testId, value, "TestID");
                _userEntryService.TestID = _testId;
            }
        }

        public string OutputDirectory
        {
            get { return _userEntryService.OutputDirectory; }
        }

        public string BatchNumber
        {
            get { return _userEntryService.BatchNumber; }
        } 
        #endregion

        private void OnTestCompleted(object obj)
        {
            //Navigate to DataEntryView
            _regionManager.RequestNavigate(RegionNames.UserDataRegion, new Uri(typeof(DataEntryViewModel).FullName, UriKind.RelativeOrAbsolute));
        }

        private void OnTestContinuedEvent(object obj)
        {
            TestID++;
        }
    }
}
