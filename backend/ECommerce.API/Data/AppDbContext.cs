using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoProduto> PedidoProdutos => Set<PedidoProduto>();
    public DbSet<Historico> Historicos => Set<Historico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(e =>
        {
            e.HasIndex(c => c.Cpf).IsUnique();
        });

        modelBuilder.Entity<Pedido>(e =>
        {
            e.HasOne(p => p.Cliente)
             .WithMany(c => c.Pedidos)
             .HasForeignKey(p => p.ClienteId);

            e.Ignore(p => p.ValorTotal);
        });

        modelBuilder.Entity<PedidoProduto>(e =>
        {
            e.HasOne(pp => pp.Pedido)
             .WithMany(p => p.Itens)
             .HasForeignKey(pp => pp.PedidoId);

            e.HasOne(pp => pp.Produto)
             .WithMany()
             .HasForeignKey(pp => pp.ProdutoId);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>().HasData(
            new Produto { Id = 1, Nome = "Notebook Dell Inspiron", Preco = 3499.90m, Descricao = "Notebook 15 polegadas, Core i5, 8GB RAM", Estoque = 10 },
            new Produto { Id = 2, Nome = "Mouse Logitech MX Master", Preco = 349.90m, Descricao = "Mouse sem fio ergonômico", Estoque = 50 },
            new Produto { Id = 3, Nome = "Teclado Mecânico Keychron K2", Preco = 599.90m, Descricao = "Teclado mecânico wireless TKL", Estoque = 30 }
        );

        modelBuilder.Entity<Cliente>().HasData(
            new Cliente { Id = 1, Nome = "João Silva", Cpf = "123.456.789-00", Email = "joao.silva@email.com" },
            new Cliente { Id = 2, Nome = "Maria Souza", Cpf = "987.654.321-00", Email = "maria.souza@email.com" }
        );
    }
}
