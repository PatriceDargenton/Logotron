
using ProductiveRage.Immutable;

namespace Bridge.React.Examples
{
	public sealed class TaskDetails : IAmImmutable
	{
		public TaskDetails(int id, string description, bool done)
		{
			this.CtorSet(_ => _.Id, id);
			this.CtorSet(_ => _.Description, description);
			this.CtorSet(_ => _.Done, done);
		}
		public int Id { get; }
		public string Description { get; }
		public bool Done { get; }
	}

    public sealed class MgsDetails : IAmImmutable
    {
        public MgsDetails(int id, string sMessage)
        {
            this.CtorSet(_ => _.Id, id);
            this.CtorSet(_ => _.Message, sMessage);
        }
        public int Id { get; }
        public string Message { get; }
    }
}