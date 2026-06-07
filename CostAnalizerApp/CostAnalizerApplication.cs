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

    // Represents a staged update (transaction/batch). Holds options and staged files.
    public class SpendingDataBatchUpdate
    {
        public UpdateOptions Options { get; }

        private readonly Dictionary<string, ISpendingCase> _stagedCases
            = new Dictionary<string, ISpendingCase>();

        public SpendingDataBatchUpdate(UpdateOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        // Stage a spending case for a file. Replaces existing entry for the same file name.
        public void AddData(string fileName, ISpendingCase spendingCase)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (spendingCase == null) throw new ArgumentNullException(nameof(spendingCase));

            _stagedCases[fileName] = spendingCase;
        }

        public IReadOnlyDictionary<string, ISpendingCase> StagedCases => _stagedCases;
    }

    public static class SpendingDataBatchUpdateFactory
    {
        public static Result<SpendingDataBatchUpdate> ValidateAndCreate(SpendingDataBatchUpdate? activeBatch, UpdateOptions? options)
        {
            if (options == null)
            {
                return Result<SpendingDataBatchUpdate>.Failure("Update options cannot be null");
            }

            if (activeBatch != null)
            {
                return Result<SpendingDataBatchUpdate>.Failure("An update is already in progress");
            }

            return Result<SpendingDataBatchUpdate>.Success(new SpendingDataBatchUpdate(options));
        }
    }
    public class CostAnalizerApplication
    {
        public SessionData Data { get; } = new SessionData();

        // Active batch (transaction)
        private SpendingDataBatchUpdate? _activeBatch;

        public bool IsUpdateInProgress => _activeBatch != null;
        // CurrentUpdateOptions removed; use the Batch returned by BeginUpdate.

        // Legacy helper that directs callers to use the batch API.
        public void AddData(string fileName, ISpendingCase spendingCase)
        {
            throw new InvalidOperationException("Use the BatchUpdate returned by BeginUpdate and call its AddData method.");
        }


        public void RemoveData(SpendingFileType type)
        {
            Data.Clear(type);
        }

        // Begin an update only if no other update is in progress. Saves update options and file type.
        public Result<SpendingDataBatchUpdate> BeginUpdate(UpdateOptions options)
        {

            if (options == null)
            {
                return Result<SpendingDataBatchUpdate>.Failure("Update options cannot be null");
            }

            if (_activeBatch != null)
            {
                return Result<SpendingDataBatchUpdate>.Failure("An update is already in progress");
            }


            var batch = new SpendingDataBatchUpdate(options);
            _activeBatch = batch;
            return Result<SpendingDataBatchUpdate>.Success(batch);
        }

        // End the current update. Accepts the batch previously returned by BeginUpdate.
        // NOTE: commit logic intentionally not implemented here.
        public Result EndUpdate(SpendingDataBatchUpdate batch)
        {
            if (batch == null) return Result.Failure("batch cannot be null");
            if (_activeBatch == null) return Result.Failure("No update in progress");
            if (!ReferenceEquals(batch, _activeBatch)) return Result.Failure("Batch does not match the active update");

            // Detach active batch (no commit implemented)
            _activeBatch = null;

            return Result.Failure("EndUpdate not implemented");
        }

        public void Clear(SpendingFileType fileType)
        {
            this.Data.Clear(fileType);
        }
    }

}
