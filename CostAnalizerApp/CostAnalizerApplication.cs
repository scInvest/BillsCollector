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
        private UpdateOptions? _currentUpdateOptions;

        public bool IsUpdateInProgress => _currentUpdateOptions != null;
        public UpdateOptions? CurrentUpdateOptions => _currentUpdateOptions;

        public void AddData(string fileName, ISpendingCase spendingCase)
        {

        }

        public void RemoveData(SpendingFileType type)
        {
            Data.Clear(type);
        }

        // Begin an update only if no other update is in progress. Saves update options and file type.
        public Result BeginUpdate(UpdateOptions options)
        {
            if (options == null)
            {
                return Result.Failure("Update options cannot be null");
            }

            if (_currentUpdateOptions != null)
            {
                return Result.Failure("An update is already in progress");
            }

            _currentUpdateOptions = options;

            return Result.Success();
        }

        // End the current update. Returns failure if no update was active.
        public Result EndUpdate()
        {
            if (_currentUpdateOptions == null)
            {
                return Result.Failure("No update in progress");
            }

            _currentUpdateOptions = null;

            return Result.Success();
        }

        public void Clear(SpendingFileType fileType)
        {
            this.Data.Clear(fileType);
        }
    }

}
