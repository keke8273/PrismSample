using System;
using System.IO;
using Microsoft.Practices.Prism.Events;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.RecordManagerModule.Services
{
    public class RecordPersistService : IRecordPersistService
    {
        private readonly IRecordTranslateService _recordTranslateService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISecurityCodeService _securityCodeService;
        private string _securityString; 

        public RecordPersistService(IRecordTranslateService recordTranslateService, IEventAggregator eventAggregator, ISecurityCodeService securityCodeService)
        {
            _recordTranslateService = recordTranslateService;
            _eventAggregator = eventAggregator;
            _securityCodeService = securityCodeService;
            _eventAggregator.GetEvent<TransientArrivedEvent>().Subscribe(o => PersistTransient(o.Transient, o.VialCaseId));
            _eventAggregator.GetEvent<ErrorDetectedEvent>().Subscribe(PersistAnalyzerFailure);
            _eventAggregator.GetEvent<TestStartedEvent>().Subscribe(CreateDataDirectory);
            _eventAggregator.GetEvent<TestCompletedEvent>().Subscribe(CreateSecurityFile);
        }

        public void PersistTransient(Transient transient, string vialCaseId)
        {
            var testType = (TestTypes) transient.Result.TestType;
            string filePath = string.Empty;
            string fileContent = string.Empty;
            switch (testType)
            {
                case TestTypes.Unknown:
                    break;
                case TestTypes.ProPTLQC:
                    fileContent = _recordTranslateService.ProteusLQCToMob(transient, vialCaseId,  out filePath);
                    break;
                case TestTypes.ProPTBlood:
                    fileContent = _recordTranslateService.ProteusPatientToMob(transient, vialCaseId, out filePath);
                    break;
                case TestTypes.RubPTLQC:
                    break;
                case TestTypes.RubPTBlood:
                    break;
                case TestTypes.RubACTLQC:
                    break;
                case TestTypes.RubACTBlood:
                    break;
                case TestTypes.RubAPTTLQC:
                    break;
                case TestTypes.RubAPTTBlood:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            using (var streamWriter = new StreamWriter(filePath, false))
            {
                streamWriter.Write(fileContent);
            }

            _securityString += _securityCodeService.CalculateSecurityCode(fileContent) + " *" +
                               Path.GetFileNameWithoutExtension(filePath) + "\n";
        }

        public void PersistAnalyzerFailure(ErrorDetectedEventArgs eventArgs)
        {
            string filePath = string.Empty;
            string fileContent = string.Empty;
            fileContent = _recordTranslateService.AnalyzerErrorToMob(eventArgs.AnalyzerFailure,eventArgs.VialCaseId, eventArgs.BuildInfo, out filePath);

            using (var streamWriter = new StreamWriter(filePath, false))
            {
                streamWriter.Write(fileContent);
            }

            _securityString += _securityCodeService.CalculateSecurityCode(fileContent) + " *" +
                               Path.GetFileNameWithoutExtension(filePath) + "\n";
        }

        private void CreateDataDirectory(object obj)
        {
            _recordTranslateService.CreateOutputDirectory();
        }

        private void CreateSecurityFile(object obj)
        {
            using (var streamWriter = new StreamWriter(_recordTranslateService.GetSecurityFilePath(), false))
            {
                streamWriter.Write(_securityString);
            }
            _securityString = string.Empty;
        }
    }
}
