﻿
using ASC.Models.Models;
using DataAccess.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business.Interface
{
    public class MasterDataOperations : IMasterDataOperations
    {
        private readonly IUnitOfWork _unitOfWork;
        public MasterDataOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<MasterDataKeyy>> GetAllMasterKeysAsync()
        {
            var masterKeys = await _unitOfWork.Responitory<MasterDataKeyy>().FindAllAsync();
            return masterKeys.ToList();
        }
        public async Task<List<MasterDataKeyy>> GetMaserKeyByNameAsync(string name)
        {
            var masterKeys = await _unitOfWork.Responitory<MasterDataKeyy>().FindAllByPartitionKeyAsync(name);
            return masterKeys.ToList();
        }
        public async Task<bool> InsertMasterKeyAsync(MasterDataKeyy key)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<MasterDataKeyy>().AddAsync(key);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<List<MasterDataValuey>> GetAllMasterValuesByKeyAsync(string key)
        {
            var masterKeys = await _unitOfWork.Responitory<MasterDataValuey>().FindAllByPartitionKeyAsync(key);
            return masterKeys.ToList();
        }
        public async Task<MasterDataValuey> GetMasterValueByNameAsync(string key, string name)
        {
            var masterValues = await _unitOfWork.Responitory<MasterDataValuey>().
           FindAsync(key, name);
            return masterValues;
        }
        public async Task<bool> InsertMasterValueAsync(MasterDataValuey value)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<MasterDataValuey>().AddAsync(value);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<bool> UpdateMasterKeyAsync(string orginalPartitionKey,
       MasterDataKeyy key)
        {
            using (_unitOfWork)
            {
                var masterKey = await _unitOfWork.Responitory<MasterDataKeyy>().
               FindAsync(orginalPartitionKey, key.RowKey);
                masterKey.IsActive = key.IsActive;
                masterKey.IsDeleted = key.IsDeleted;
                masterKey.Name = key.Name;
                await _unitOfWork.Responitory<MasterDataKeyy>().UpdateAsync(masterKey);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<bool> UpdateMasterValueAsync(string originalPartitionKey, string
       originalRowKey, MasterDataValuey value)
        {
            using (_unitOfWork)
            {
                var masterValue = await _unitOfWork.Responitory<MasterDataValuey>().
               FindAsync(originalPartitionKey, originalRowKey);
                masterValue.IsActive = value.IsActive;
                masterValue.IsDeleted = value.IsDeleted;
                masterValue.Name = value.Name;
                await _unitOfWork.Responitory<MasterDataValuey>().UpdateAsync(masterValue);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<List<MasterDataValuey>> GetAllMasterValuesAsync()
        {
            var masterValues = await _unitOfWork.Responitory<MasterDataValuey>().
           FindAllAsync();
            return masterValues.ToList();
        }
        public async Task<bool> UploadBulkMasterData(List<MasterDataValuey> values)
        {
            using (_unitOfWork)
            {
                foreach (var value in values)
                {
                    // Find, if null insert MasterKey
                    var masterKey = await GetMaserKeyByNameAsync(value.PartitionKey);
                    if (!masterKey.Any())
                    {
                        await _unitOfWork.Responitory<MasterDataKeyy>().AddAsync(new MasterDataKeyy()
                        {
                            Name = value.PartitionKey,
                            RowKey = Guid.NewGuid().ToString(),
                            PartitionKey = value.PartitionKey
                        });
                    }
                    // Find, if null Insert MasterValue
                    var masterValuesByKey = await GetAllMasterValuesByKeyAsync(value.PartitionKey);
                    var masterValue = masterValuesByKey.FirstOrDefault(p => p.Name == value.Name);
                    if (masterValue == null)
                    {
                        await _unitOfWork.Responitory<MasterDataValuey>().AddAsync(value);
                    }
                    else
                    {
                        masterValue.IsActive = value.IsActive;
                        masterValue.IsDeleted = value.IsDeleted;
                        masterValue.Name = value.Name;
                        await _unitOfWork.Responitory<MasterDataValuey>().UpdateAsync(masterValue);
                    }
                }
                _unitOfWork.CommitTransaction();
                return true;
            }
        }

        public Task<bool> UploadBulkMasterData(List<MasterDataKeyy> values)
        {
            throw new NotImplementedException();
        }
    }
}
