using System;

namespace Excel.Domain
{
    public class Retorno
    {
        public int Codigo { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataProcessamento { get; set; }
    }
}