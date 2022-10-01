namespace Giantnodes.Infrastructure.Abstractions
{
    public interface ITimestampableEntity
    {
        DateTime? UpdatedAt { get; set; }

        DateTime CreatedAt { get; set; }
    }
}
