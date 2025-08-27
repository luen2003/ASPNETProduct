using ServiceAPI.Models.TransactionRequest;
using ServiceAPI.Models.TransactionResponse;

namespace ServiceAPI.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        public Task<List<TransactionCommandDataGo>> GetDatabyCommand(int commandCode);
        public Task<List<TransactionCommandDataGo>> GetDataTransactionbyCommand(string fromdate, string todate, List<int> commandCode, List<string> warehouse);
        public Task<CommmandResponseTo> SaveTransactionCode(CommmandRequestTo commmandRequest);

        public Task<CommmandResponseTo> SaveTransactionCodeDOPToDOP1(CommmandRequestTo commmandRequest);
        public Task<string> UpdateVersionByCommandCode(int commandCode);

    }
}
