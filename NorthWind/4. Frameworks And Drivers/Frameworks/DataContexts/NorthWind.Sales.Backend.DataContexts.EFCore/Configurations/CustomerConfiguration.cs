namespace NorthWind.Sales.Backend.DataContexts.EFCore.Configurations;

internal class CustomerConfiguration :
    IEntityTypeConfiguration<Repositories.Entities.Customer>
{
    public void Configure(EntityTypeBuilder<Repositories.Entities.Customer> builder)
    {
        // Up to 13 characters to allow Ecuadorian national id numbers.
        builder
            .Property(c => c.Id)
            .HasMaxLength(13);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(c => c.CurrentBalance)
            .HasPrecision(8, 2);

        builder.HasData(
            new Repositories.Entities.Customer
            {
                Id = "ALFKI",
                Name = "Alfreds Futterkiste",
                CurrentBalance = 0
            },
            new Repositories.Entities.Customer
            {
                Id = "ANATR",
                Name = "Ana Trujillo Emparedados y helados",
                CurrentBalance = 0
            },
            new Repositories.Entities.Customer
            {
                Id = "ANTON",
                Name = "Antonio Moreno Taquería",
                CurrentBalance = 100
            });
    }
}
