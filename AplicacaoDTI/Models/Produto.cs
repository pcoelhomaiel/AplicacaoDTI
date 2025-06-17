namespace AplicacaoDTI.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public decimal? PrecoUnitario { get; set; }
        public bool Excluido { get; set; } = false;

        public ICollection<ItemEstoque>? Estoques { get; set; }

    }
}
