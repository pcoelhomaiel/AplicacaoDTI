using AplicacaoDTI.Models;

namespace AplicacaoDTI.Models
{
    public class ItemEstoque
    {
        public int IdProduto { get; set; }
        public Produto? Produto { get; set; }
        public int IdLoja { get; set; }
        public Loja? Loja { get; set; }
        public int Quantidade { get; set; }
    }
}