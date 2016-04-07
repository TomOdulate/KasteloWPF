using System;

namespace Tao.CredentialStore
{
    public interface IItemStore
    {
        string Name { get; set; }
        bool Add(IStoreableItem item);
        bool Remove(IStoreableItem item);
        int Count();
    }

    public interface IStoreableItem
    {
        DateTime Created { get; set; }
        DateTime LastUpdated { get; set; }
    }
}
