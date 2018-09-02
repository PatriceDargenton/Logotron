
using System;
using System.Collections.Generic;

namespace Bridge.React.Logotron.API
{
    public sealed class ItemApi : IReadAndWriteItems
    {
        private readonly List<Tuple<int, string>> _messages;
        private readonly List<string> _lstsMessages;
        public ItemApi(List<string> lstItems)
        {
            _messages = new List<Tuple<int, string>>();
            _lstsMessages = lstItems;
            for (int i = 0; i < lstItems.Count; i++)
            {
                _messages.Add(Tuple.Create(i, lstItems[i]));
            }
        }
        public IEnumerable<Tuple<int, string>> GetItems() { return _messages; }
        public List<string> GetItemList() { return _lstsMessages; }
        public string GetItem(int i) { return _lstsMessages[i]; }
    }
}
