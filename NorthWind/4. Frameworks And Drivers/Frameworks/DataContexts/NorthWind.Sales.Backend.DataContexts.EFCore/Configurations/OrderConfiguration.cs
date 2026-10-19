namespace NorthWind.Sales.Backend.DataContexts.EFCore.Configurations;

internal class OrderConfiguration :
    IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Up to 13 characters to allow Ecuadorian national id numbers.
        builder.Property(o => o.CustomerId)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(o => o.Subtotal)
            .HasPrecision(10, 2);

        builder.Property(o => o.Tax)
            .HasPrecision(10, 2);

        builder.Property(o => o.Total)
            .HasPrecision(10, 2);

        builder.Property(o => o.ShipAddress)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(o => o.ShipCity)
            .HasMaxLength(15);

        builder.Property(o => o.ShipCountry)
            .HasMaxLength(15);

        builder.Property(o => o.ShipPostalCode)
            .HasMaxLength(10);

        builder.HasOne<Repositories.Entities.Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId);
    }
}
