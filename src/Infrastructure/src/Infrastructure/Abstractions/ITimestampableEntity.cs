namespace Giantnodes.Infrastructure.Abstractions
{
    public interface ITimestampableEntity
    {
        DateTime CreatedAt { get; set; }

        DateTime? UpdatedAt { get; set; }
    }
}
