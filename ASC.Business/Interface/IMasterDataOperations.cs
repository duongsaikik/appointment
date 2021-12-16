using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business.Interface
{
    public interface IMasterDataOperations
    {
        Task<List<MasterDataKeyy>> GetAllMasterKeysAsync();
        Task<List<MasterDataKeyy>> GetMaserKeyByNameAsync(string name);
        Task<bool> InsertMasterKeyAsync(MasterDataKeyy key);
        Task<bool> UpdateMasterKeyAsync(string orginalPartitionKey, MasterDataKeyy key);
        Task<List<MasterDataValuey>> GetAllMasterValuesByKeyAsync(string key);
        Task<List<MasterDataValuey>> GetAllMasterValuesAsync();
        Task<MasterDataValuey> GetMasterValueByNameAsync(string key, string name);
        Task<bool> InsertMasterValueAsync(MasterDataValuey value);
        Task<bool> UpdateMasterValueAsync(string originalPartitionKey, string originalRowKey, MasterDataValuey value);
        Task<bool> UploadBulkMasterData(List<MasterDataKeyy> values);
    }
}
