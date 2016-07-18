using System;
using System.IO;
using System.Text;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;

namespace QBR.RecordManagerModule.Services
{
    public class RecordTranslateService : IRecordTranslateService
    {
        private readonly IUserEntryService _userEntryService;
        private readonly ISoftwareUpgradeService _softwareUpgradeService;
        private readonly ITimeStampService _timeStampService;
        private readonly IAnalyzerConfigurationService _analyzerConfigurationService;

        public RecordTranslateService(IUserEntryService userEntryService, ISoftwareUpgradeService softwareUpgradeService, ITimeStampService timeStampService, IAnalyzerConfigurationService analyzerConfigurationService)
        {
            _userEntryService = userEntryService;
            _softwareUpgradeService = softwareUpgradeService;
            _timeStampService = timeStampService;
            _analyzerConfigurationService = analyzerConfigurationService;
        }

        public void CreateOutputDirectory()
        {
            Directory.CreateDirectory(Path.Combine(_userEntryService.OutputDirectory, _userEntryService.BatchNumber));
        }

        public string ProteusPatientToMob(Transient transient, string vialCaseId, out string filePath)
        {
            TransientResultPatient transientResult;
            SpecificPatientTRData specificPatientData;
            filePath = Path.Combine(_userEntryService.OutputDirectory, _userEntryService.BatchNumber, GetFileName(_userEntryService.BankID.ToString(), _analyzerConfigurationService.GetAnalyzerID(transient.GetSerialNumber()).ToString(), ".mob"));

            try
            {
                transientResult = transient.Result as TransientResultPatient;
                specificPatientData = transientResult.Patient.SpecificData as SpecificPatientTRData;
            }
            catch (Exception)
            {
                throw;
            }

            var transientData = transient.TransientData;

            var mobString = new StringBuilder();
            mobString.Append("Device Type: PROTEUS METER\r\n");
            mobString.Append("Data Type: Live Data\r\n");
            mobString.Append("\r\n");

            mobString.AppendFormat("Operator: {0}\r\n", _userEntryService.OperatorID);
            mobString.AppendFormat("File: {0}\r\n", filePath);
            mobString.AppendFormat("File Date and Time: {0}\r\n", _timeStampService.TimeStamp);
            mobString.AppendFormat("Software Version: {0}\r\n", _softwareUpgradeService.CurrentVersion);
            mobString.Append("\r\n");

            mobString.Append("SAMPLE DESCRIPTION\r\n");
            mobString.AppendFormat("Case /Vial Number: {0}\r\n", vialCaseId);
            mobString.AppendFormat("Replicate Number: {0}\r\n", transientResult.AccessionNumber);
            mobString.AppendFormat("Target HCT (%): {0}\r\n", _userEntryService.TargetHCT);
            mobString.AppendFormat("Target INR (Units2): {0}\r\n", _userEntryService.TargetINR);
            mobString.AppendFormat("Donor / Solution ID: {0}\r\n", _userEntryService.TestTarget);
            mobString.AppendFormat("Strip Lot ID: {0}\r\n", _userEntryService.BatchNumber);
            mobString.AppendFormat("Sample Type: {0}\r\n", transientResult.Patient.Sample.TypeFlag);
            mobString.AppendFormat("Calibration Strip Data: {0}\r\n", string.Empty);
            mobString.AppendFormat("Calibration Vial Data: {0}\r\n", string.Empty);
            mobString.Append("\r\n");

            mobString.Append("METER DESCRIPTION\r\n");
            mobString.AppendFormat("Meter ID: {0}\r\n", _analyzerConfigurationService.GetAnalyzerID(transient.GetSerialNumber()));
            mobString.AppendFormat("Meter Serial Number: {0}\r\n", transientResult.Patient.BuildInformation.SerialNumber);
            mobString.AppendFormat("Meter Hardware Version: {0}\r\n", transientResult.Patient.BuildInformation.HWRelease);
            mobString.AppendFormat("Meter Software Version: {0}\r\n", transientResult.Patient.BuildInformation.SWVersion);
            mobString.AppendFormat("Test Date and Time: {0}\r\n", string.Empty);
            mobString.AppendFormat("Clot Time (s): {0}\r\n", transientResult.Patient.TestResult.CT);
            mobString.AppendFormat("Partial Fill: {0}\r\n", string.Empty);
            mobString.AppendFormat("Double Fill: {0}\r\n", string.Empty);
            mobString.AppendFormat("OBC Value: {0}\r\n", transientResult.Patient.Transient.OBCValue);
            mobString.AppendFormat("Minimum Current: {0}\r\n", transientResult.Patient.Transient.Minimum_Current);
            mobString.AppendFormat("Error Code: {0}\r\n", transientResult.Patient.FaultIdentifier);
            mobString.Append("\r\n");

            mobString.Append("COMMENTS:\r\n");
            mobString.Append(_userEntryService.Comments);
            mobString.Append("\r\n");

            mobString.Append("DATA ARRAYS\r\n");
            mobString.Append("Time (sec)\tCurrent (uA)\r\n");

            var timeStamp = Properties.Settings.Default.ProteusStartTime;
            for (var i = 0; i < transientData.TransientSize; i++)
            {
                mobString.Append(string.Format("{0:N5}\t{1}\r\n", timeStamp, transientData.TransientStripCurrents[i] / 1000));
                timeStamp += Properties.Settings.Default.ProteusSamplingInterval;
            }
            mobString.Append("End of File");
            return mobString.ToString();
        }

        public string ProteusLQCToMob(Transient transient, string vialCaseId, out string filePath)
        {
            TransientResultLQC transientResult;
            SpecificLQCTRData specificData;
            filePath = Path.Combine(_userEntryService.OutputDirectory, 
                _userEntryService.BatchNumber, GetFileName(_userEntryService.BankID.ToString(), 
                _analyzerConfigurationService.GetAnalyzerID(transient.GetSerialNumber()).ToString(), ".mob"));

            try
            {
                transientResult = transient.Result as TransientResultLQC;
                specificData = transientResult.LQC.SpecificData as SpecificLQCTRData;
            }
            catch (Exception)
            {
                throw;
            }

            var transientData = transient.TransientData;

            var mobString = new StringBuilder();
            mobString.Append("Device Type: PROTEUS METER\r\n");
            mobString.Append("Data Type: Live Data\r\n");
            mobString.Append("\r\n");

            mobString.AppendFormat("Operator: {0}\r\n", _userEntryService.OperatorID);
            mobString.AppendFormat("File: {0}\r\n", filePath);
            mobString.AppendFormat("File Date and Time: {0}\r\n", _timeStampService.TimeStamp);
            mobString.AppendFormat("Software Version: {0}\r\n", _softwareUpgradeService.CurrentVersion);
            mobString.Append("\r\n");

            mobString.Append("SAMPLE DESCRIPTION\r\n");
            mobString.AppendFormat("Case /Vial Number: {0}\r\n", vialCaseId);
            mobString.AppendFormat("Replicate Number: {0}\r\n", "1");
            mobString.AppendFormat("Target HCT (%): {0}\r\n", _userEntryService.TargetHCT);
            mobString.AppendFormat("Target INR (Units2): {0}\r\n", _userEntryService.TargetINR);
            mobString.AppendFormat("Donor / Solution ID: {0}\r\n", _userEntryService.TestTarget);
            mobString.AppendFormat("Strip Lot ID: {0}\r\n", _userEntryService.BatchNumber);
            mobString.AppendFormat("Sample Type: {0}\r\n", transientResult.LQC.Sample.TypeFlag);
            mobString.AppendFormat("Calibration Strip Data: {0}\r\n", string.Empty);
            mobString.AppendFormat("Calibration Vial Data: {0}\r\n", string.Empty);
            mobString.Append("\r\n");

            mobString.Append("METER DESCRIPTION\r\n");
            mobString.AppendFormat("Meter ID: {0}\r\n", _analyzerConfigurationService.GetAnalyzerID(transient.GetSerialNumber()));
            mobString.AppendFormat("Meter Serial Number: {0}\r\n", transientResult.LQC.BuildInformation.SerialNumber);
            mobString.AppendFormat("Meter Hardware Version: {0}\r\n", transientResult.LQC.BuildInformation.HWRelease);
            mobString.AppendFormat("Meter Software Version: {0}\r\n", transientResult.LQC.BuildInformation.SWVersion);
            mobString.AppendFormat("Test Date and Time: {0}\r\n", string.Empty);
            mobString.AppendFormat("Average Test Temperature: {0}\r\n", string.Empty);
            mobString.AppendFormat("INR: {0}\r\n", transientResult.LQC.TestResult.INR);
            mobString.AppendFormat("Clot Time (s): {0}\r\n", transientResult.LQC.TestResult.CT);
            //mobString.AppendFormat("Calibrated Clot Time (s): {0}\r\n", transientResult.Patient.TestResult.CCT);
            mobString.AppendFormat("Control Reaction (uA.sec): {0}\r\n", transientResult.LQC.Transient.OBCValue);
            //mobString.AppendFormat("Minimum Current (uA): {0}\r\n", transientResult.Patient.Transient.Minimum_Current * 1000000);
            mobString.AppendFormat("Error Code: {0}\r\n", transientResult.LQC.FaultIdentifier);
            mobString.Append("\r\n");

            mobString.Append("COMMENTS:\r\n");
            mobString.Append(_userEntryService.Comments);
            mobString.Append("\r\n");

            mobString.Append("DATA ARRAYS\r\n");
            mobString.Append("Time (sec)\tCurrent (uA)\r\n");

            var timeStamp = Properties.Settings.Default.ProteusStartTime;
            for (var i = 0; i < transientData.TransientSize; i++)
            {
                mobString.Append(string.Format("{0:N5}\t{1}\r\n", timeStamp, transientData.TransientStripCurrents[i] / 1000));
                timeStamp += Properties.Settings.Default.ProteusSamplingInterval;
            }
            mobString.Append("End of File");
            return mobString.ToString();
        }

        public string AnalyzerErrorToMob(AnalyzerFailure analyzerFailure, string vialCaseId, BuildInfo buildInfo, out string filePath)
        {
            filePath = Path.Combine(_userEntryService.OutputDirectory, _userEntryService.BatchNumber, 
                GetFileName(_userEntryService.BankID.ToString(), 
                _analyzerConfigurationService.GetAnalyzerID(buildInfo.SerialNumber).ToString(), ".mob"));

            var mobString = new StringBuilder();
            mobString.Append("Device Type: PROTEUS METER\r\n");
            mobString.Append("Data Type: Live Data\r\n");
            mobString.Append("\r\n");

            mobString.AppendFormat("Operator: {0}\r\n", _userEntryService.OperatorID);
            mobString.AppendFormat("File: {0}\r\n", filePath);
            mobString.AppendFormat("File Date and Time: {0}\r\n", _timeStampService.TimeStamp);
            mobString.AppendFormat("Software Version: {0}\r\n", _softwareUpgradeService.CurrentVersion);
            mobString.Append("\r\n");

            mobString.Append("SAMPLE DESCRIPTION\r\n");
            mobString.AppendFormat("Case /Vial Number: {0}\r\n", vialCaseId);
            mobString.AppendFormat("Replicate Number: {0}\r\n", "1");
            mobString.AppendFormat("Target HCT (%): {0}\r\n", _userEntryService.TargetHCT);
            mobString.AppendFormat("Target INR (Units2): {0}\r\n", _userEntryService.TargetINR);
            mobString.AppendFormat("Donor / Solution ID: {0}\r\n", _userEntryService.TestTarget);
            mobString.AppendFormat("Strip Lot ID: {0}\r\n", _userEntryService.BatchNumber);
            mobString.AppendFormat("Sample Type: {0}\r\n", string.Empty);
            mobString.AppendFormat("Calibration Strip Data: {0}\r\n", string.Empty);
            mobString.AppendFormat("Calibration Vial Data: {0}\r\n", string.Empty);
            mobString.Append("\r\n");

            mobString.Append("METER DESCRIPTION\r\n");
            mobString.AppendFormat("Meter ID: {0}\r\n", _analyzerConfigurationService.GetAnalyzerID(buildInfo.SerialNumber));
            mobString.AppendFormat("Meter Serial Number: {0}\r\n", buildInfo.SerialNumber);
            mobString.AppendFormat("Meter Hardware Version: {0}\r\n", buildInfo.HWRelease);
            mobString.AppendFormat("Meter Software Version: {0}\r\n", buildInfo.SWVersion);
            mobString.AppendFormat("Test Date and Time: {0}\r\n", string.Empty);
            mobString.AppendFormat("Average Test Temperature: {0}\r\n", string.Empty);
            mobString.AppendFormat("INR: {0}\r\n", "N/A");
            mobString.AppendFormat("Clot Time (s): {0}\r\n", "N/A");
            //mobString.AppendFormat("Calibrated Clot Time (s): {0}\r\n", transientResult.Patient.TestResult.CCT);
            mobString.AppendFormat("Control Reaction (uA.sec): {0}\r\n", "N/A");
            //mobString.AppendFormat("Minimum Current (uA): {0}\r\n", transientResult.Patient.Transient.Minimum_Current * 1000000);
            mobString.AppendFormat("Error Code: {0}\r\n", analyzerFailure);
            mobString.Append("\r\n");

            mobString.Append("COMMENTS:\r\n");
            mobString.Append(_userEntryService.Comments);
            mobString.Append("\r\n");

            mobString.Append("DATA ARRAYS\r\n");
            mobString.Append("Time (sec)\tCurrent (uA)\r\n");
            mobString.Append("End of File");
            return mobString.ToString();
        }

        public string GetSecurityFilePath()
        {
            return Path.Combine(_userEntryService.OutputDirectory, _userEntryService.BatchNumber, GetFileName(_userEntryService.BankID.ToString(), string.Empty, ".md5"));
        }

        private string GetFileName(string bankID, string analyzerID, string extension)
        {
            var fileNameBuilder = new StringBuilder();

            fileNameBuilder.Append("PROBR");
            fileNameBuilder.Append(_timeStampService.TimeStamp.ToString("yyMMdd"));
            fileNameBuilder.Append("-");
            fileNameBuilder.Append(_userEntryService.BatchNumber);
            fileNameBuilder.Append("_");
            fileNameBuilder.Append(string.Format("B{0}", bankID));
            fileNameBuilder.Append("_");
            fileNameBuilder.Append(_userEntryService.TestID.ToString("D4"));
            fileNameBuilder.Append(analyzerID);
            fileNameBuilder.Append(extension);

            return fileNameBuilder.ToString();
        }
    }
}
