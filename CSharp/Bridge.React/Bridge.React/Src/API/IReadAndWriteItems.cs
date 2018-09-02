
using System; // Tuple
using System.Collections.Generic; // IEnumerable
using System.Threading.Tasks; // Task

namespace Bridge.React.Logotron.API
{
    public interface IReadAndWriteItems
    {
        IEnumerable<Tuple<int, string>> GetItems();
        List<string> GetItemList();
        string GetItem(int i);
    }

    public interface IReadAndWriteMessages
    {
        Task SaveMessage(MessageDetails message);
        Task SaveMessage(string message);
        Task Attendre(int iDelaiMsec);
    }
}
