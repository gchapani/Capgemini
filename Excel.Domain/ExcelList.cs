using System;

namespace Excel.Domain
{
    public class ExcelList
    {
        public DateTime DataEntrega { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Id { get; set; }
    }
}