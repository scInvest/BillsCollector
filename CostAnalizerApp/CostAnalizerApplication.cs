using CostAnalizerApp.Interfaces;
using DataSrouce.API;
using SharedCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace CostAnalizerApp
{
    public enum UpdateType
    {
        Replace
    }

    public class UpdateOptions
    {
        public required UpdateType UpdateType { get; init; }
        public required SpendingFileType SpendingFileType { get; init; }
    }

    public class CostAnalizerApplication
    {
        public SessionData Data { get; } = new SessionData();

        // Simple single-update guard
        private bool _updateInProgress;
        private UpdateType? _currentUpdateType;
        private SpendingFileType? _currentSpendingFileType;

        public bool IsUpdateInProgress => _updateInProgress;
        public UpdateType? CurrentUpdateType => _currentUpdateType;
        public SpendingFileType? CurrentSpendingFileType => _currentSpendingFileType;

        public void AddData(string fileName, ISpendingCase spendingCase)
        {
        }


        // Begin an update only if no other update is in progress. Saves update options and file type.
        public Result BeginUpdate(UpdateOptions options)
        {
            if (options == null)
            {
                return Result.Failure("Update options cannot be null");
            }

            if (_updateInProgress)
            {
                return Result.Failure("An update is already in progress");
            }

            _updateInProgress = true;
            _currentUpdateType = options.UpdateType;
            _currentSpendingFileType = options.SpendingFileType;

            return Result.Success();
        }

        // End the current update. Returns failure if no update was active.
        public Result EndUpdate()
        {
            if (!_updateInProgress)
            {
                return Result.Failure("No update in progress");
            }

            _updateInProgress = false;
            _currentUpdateType = null;
            _currentSpendingFileType = null;

            return Result.Success();
        }

        public void Clear(SpendingFileType fileType)
        {
            this.Data.Clear(fileType);
        }
    }

}
