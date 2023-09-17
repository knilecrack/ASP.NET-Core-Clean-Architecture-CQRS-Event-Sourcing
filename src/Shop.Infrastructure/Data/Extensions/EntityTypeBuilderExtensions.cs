using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.SharedKernel;

namespace Shop.Infrastructure.Data.Extensions;

internal static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Configures the base entity for a given entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="builder">The entity type builder.</param>
    internal static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        // Sets the primary key for the entity to the Id property.
        builder
            .HasKey(entity => entity.Id);

        // Configures the Id property to be required (NOT NULL) and not generated by the database.
        builder
            .Property(entity => entity.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Ignores the DomainEvents property for the entity.
        builder
            .Ignore(entity => entity.DomainEvents);
    }
}
