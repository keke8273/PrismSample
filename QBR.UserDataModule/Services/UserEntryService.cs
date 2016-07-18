using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.Enums;

namespace QBR.UserDataModule.Services
{
    public class UserEntryService : IUserEntryService
    {
        private int _bankID;
        private int _testID;
        private string _operatorID;
        private string _batchNumber;
        private string _testTarget;
        private StripType _stripType;
        private double _targetINR;
        private double _targetHCT;
        private bool _hasError;
        public UserEntryService()
        {
            RecentOutputDirectories = Properties.Settings.Default.RecentOutputDirectories;
        }

        public int BankID
        {
            get { return _bankID; }
            set
            {
                _bankID = value;
                UserEntryUpdated.Raise(this);
            }
        }

        public int TestID
        {
            get { return _testID; }
            set
            {
                _testID = value;
                UserEntryUpdated.Raise(this);
            }
        }

        public string OperatorID
        {
            get { return _operatorID; }
            set
            {
                _operatorID = value;
                UserEntryUpdated.Raise(this);
            }
        }
        public string OutputDirectory
        {
            get { return RecentOutputDirectories[0]; }
            set { UpdateRecentOutputDirectories(value);}
        }

        public string BatchNumber
        {
            get { return _batchNumber; }
            set
            {
                _batchNumber = value;
                UserEntryUpdated.Raise(this);
            }
        }

        public StripType StripType
        {
            get { return _stripType; }
            set
            {
                _stripType = value;
                UserEntryUpdated.Raise(this);
            }
        }

        public string TestTarget
        {
            get { return _testTarget; }
            set
            {
                _testTarget = value;
                UserEntryUpdated.Raise(this);
            }
        }

        public double TargetINR
        {
            get { return _targetINR; }
            set
            {
                _targetINR = value;
                UserEntryUpdated.Raise(this);
            }
        }

        public double TargetHCT
        {
            get { return _targetHCT; }
            set
            {
                _targetHCT = value;
                UserEntryUpdated.Raise(this);
            }
        }

        public bool HasError
        {
            get { return _hasError; }
            set
            {
                _hasError = value;
                UserEntryUpdated.Raise(this);
            }
        }

        public string Comments { get; set; }

        public StringCollection RecentOutputDirectories { get; set; }

        public bool IsAllDataCollected()
        {
            if (string.IsNullOrEmpty(OperatorID))
                return false;

            if (TestID <= 0)
                return false;

            if (string.IsNullOrEmpty(OutputDirectory))
                return false;

            if (string.IsNullOrEmpty(BatchNumber))
                return false;

            if (BankID <= 0)
                return false;

            if (StripType == StripType.Unkown)
                return false;

            if (string.IsNullOrEmpty(TestTarget))
                return false;

            return true;
        }

        public event EventHandler UserEntryUpdated;

        private void UpdateRecentOutputDirectories(string newOutputDirectory)
        {
            if (RecentOutputDirectories.Contains(newOutputDirectory))
            {
                RecentOutputDirectories.Remove(newOutputDirectory);
            }

            RecentOutputDirectories.Insert(0, newOutputDirectory);

            if (RecentOutputDirectories.Count > Properties.Settings.Default.MaxRecentOutputDirectoryCount)
            {
                RecentOutputDirectories.RemoveAt(Properties.Settings.Default.MaxRecentOutputDirectoryCount);
            }

            UserEntryUpdated.Raise(this);
        }
    }
}
