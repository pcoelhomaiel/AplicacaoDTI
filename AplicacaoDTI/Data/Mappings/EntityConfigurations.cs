using AplicacaoDTI.Models;
using Microsoft.EntityFrameworkCore;

namespace AplicacaoDTI.Data.Mappings
{
    public static class EntityConfigurations
    {
        public static void Mapeamento(this ModelBuilder modelBuilder)
        {
            //PRODUTO
            modelBuilder.Entity<Produto>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Produto>()
                .HasQueryFilter(p => !p.Excluido);

            modelBuilder.Entity<Produto>()
                .Property(p => p.PrecoUnitario)
                .HasPrecision(10, 2);

            //LOJA
            modelBuilder.Entity <Loja>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Loja>()
                .HasQueryFilter(l => !l.Excluido);

            //ITEMESTOQUE
            modelBuilder.Entity<ItemEstoque>()
                .HasKey(e => new { e.IdProduto, e.IdLoja }); //Chave Composta

            modelBuilder.Entity<ItemEstoque>()
                .HasQueryFilter(e => !e.Produto.Excluido && !e.Loja.Excluido);

            modelBuilder.Entity<ItemEstoque>()
                .HasOne(e => e.Produto)            // ← ItemEstoque tem uma navegação: public Produto Produto { get; set; }
                .WithMany(p => p.Estoques)         // ← Produto tem: public ICollection<ItemEstoque> Estoques { get; set; }
                .HasForeignKey(e => e.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemEstoque>()
                .HasOne(e => e.Loja)            // ← ItemEstoque tem uma navegação: public Loja Loja { get; set; }
                .WithMany(l => l.Estoques)         // ← Loja tem: public ICollection<ItemEstoque> Estoques { get; set; }
                .HasForeignKey(e => e.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

