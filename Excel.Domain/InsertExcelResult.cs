using System;

namespace Excel.Domain
{
    public class InsertExcelResult
    {
        public Retorno Retorno { get; set; }
        public int Id { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime DataImportacao { get; set; }
        public long TotalRegistros { get; set; }

        public InsertExcelResult()
        {
            Retorno = new Retorno();
        }
    }
}