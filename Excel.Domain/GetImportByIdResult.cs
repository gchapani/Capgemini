using System;

namespace Excel.Domain
{
    public class GetImportByIdResult
    {
        public Retorno Retorno { get; set; }
        public DateTime DataEntrega { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}