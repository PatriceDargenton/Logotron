
using ProductiveRage.Immutable;

namespace Bridge.React.Logotron.API
{
    public sealed class SavedMessageDetails : IAmImmutable
    {
        public SavedMessageDetails(uint id, MessageDetails message)
        {
            this.CtorSet(_ => _.Id, id);
            this.CtorSet(_ => _.Message, message);
        }
        public uint Id { get; }
        public MessageDetails Message { get; }
    }
}
