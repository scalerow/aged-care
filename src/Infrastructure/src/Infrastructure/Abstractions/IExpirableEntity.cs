namespace Giantnodes.Infrastructure.Abstractions
{
    public interface IExpirableEntity
    {
        public DateTime ExpiresAt { get; set; }
    }
}
