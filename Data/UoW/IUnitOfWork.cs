using System.Data;

namespace Data.UoW
{
    public interface IUnitOfWork
    {
        void ChangeDbConnectionString(string connectionString);
        int Complete();
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void CommitTrans();
        void RollBackTrans();
    }
}
