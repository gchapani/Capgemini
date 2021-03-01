using System;

namespace Excel.Domain
{
    public class GetAllImportsResult
    {
        public Retorno Retorno { get; set; }
        public int Id { get; set; }
        public DateTime DataImportacao { get; set; }
        public int NumeroItens { get; set; }
        public DateTime DataEntrega { get; set; }
        public decimal ValorTotalImportacao { get; set; }
    }
}