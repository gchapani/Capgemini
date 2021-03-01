using System;

namespace Excel.Data.Models
{
    public class IMPORTACAO_EXCEL
    {
        public int Id { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime DataImportacao { get; set; }
    }
}