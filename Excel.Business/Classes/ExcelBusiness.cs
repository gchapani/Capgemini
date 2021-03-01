using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Excel.Business.Interfaces;
using Excel.Data.Context;
using Excel.Data.Models;
using Excel.Domain;
using Excel.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using ExcelDataReader;
using SQLitePCL;
using System.Net.Http;
using System.Web.Mvc;

namespace Excel.Business.Classes
{
    public class ExcelBusiness : IExcelBusiness
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ExcelContext> _unitOfWork;

        public ExcelBusiness(IMapper mapper, IUnitOfWork<ExcelContext> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<GetAllImportsResult> GetAllImports()
        {
            List<GetAllImportsResult> oGetAllImportsResult = new List<GetAllImportsResult>();
            GetAllImportsResult _GetAllImportsResult = new GetAllImportsResult();

            try
            {
                List<IMPORTACAO_EXCEL> oIMPORTACAO_EXCEL = _unitOfWork.Repository<IMPORTACAO_EXCEL>().GetAll().Result.ToList();
                List<EXCEL> oEXCEL = _unitOfWork.Repository<EXCEL>().GetAll().Result.ToList();

                foreach (IMPORTACAO_EXCEL item in oIMPORTACAO_EXCEL)
                {
                    _GetAllImportsResult.Id = item.Id;
                    _GetAllImportsResult.DataImportacao = item.DataImportacao;
                    _GetAllImportsResult.NumeroItens = oEXCEL.Where(x => x.Id == item.Id).Count();
                    _GetAllImportsResult.DataEntrega = oEXCEL.Where(x => x.Id == item.Id).Select(x => x.DataEntrega).Min();
                    _GetAllImportsResult.ValorTotalImportacao = oEXCEL.Where(x => x.Id == item.Id).Select(x => x.ValorUnitario * x.Quantidade).Sum();

                    oGetAllImportsResult.Add(_GetAllImportsResult);
                }
            }
            catch (Exception ex)
            {
                _GetAllImportsResult.Retorno = new Retorno();

                _GetAllImportsResult.Retorno.Codigo = 9999;
                _GetAllImportsResult.Retorno.Mensagem = $"GetAllImports: {ex.Message}";
                _GetAllImportsResult.Retorno.DataProcessamento = DateTime.Now;

                oGetAllImportsResult.Add(_GetAllImportsResult);
            }

            return oGetAllImportsResult;
        }
        public List<GetImportByIdResult> GetImportById(int id)
        {
            List<GetImportByIdResult> oGetImportByIdResult = new List<GetImportByIdResult>();
            GetImportByIdResult _GetImportByIdResult = new GetImportByIdResult();

            try
            {
                List<EXCEL> oEXCEL = _unitOfWork.Repository<EXCEL>().GetAll().Result.Where(x => x.Id == id).ToList();

                oGetImportByIdResult = _mapper.Map<List<GetImportByIdResult>>(oEXCEL);
                oGetImportByIdResult.Select(x => { x.ValorTotal = x.Quantidade * x.ValorUnitario; return x; }).ToList();

                return oGetImportByIdResult;
            }
            catch (Exception ex)
            {
                _GetImportByIdResult.Retorno = new Retorno();

                _GetImportByIdResult.Retorno.Codigo = 9999;
                _GetImportByIdResult.Retorno.Mensagem = $"GetImportById({id}): {ex.Message}";
                _GetImportByIdResult.Retorno.DataProcessamento = DateTime.Now;

                oGetImportByIdResult.Add(_GetImportByIdResult);
            }

            return oGetImportByIdResult;
        }
        public async Task<InsertExcelResult> Insert(IFormFile arquivo)
        {
            #region Variáveis
            GetAllImportsResult _GetAllImportsResult = new GetAllImportsResult();
            InsertExcelResult _InsertExcelResult = new InsertExcelResult();
            IMPORTACAO_EXCEL _IMPORTACAO_EXCEL = new IMPORTACAO_EXCEL();
            List<EXCEL> oEXCEL = new List<EXCEL>();
            #endregion

            try
            {
                #region Validações do Arquivo
                if (arquivo.Length == 0)
                {
                    _InsertExcelResult.Retorno.Codigo = 1010;
                    _InsertExcelResult.Retorno.Mensagem = "Arquivo está vazio";
                    return _InsertExcelResult;
                }

                string fileExtension = Path.GetExtension(arquivo.FileName);

                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    _InsertExcelResult.Retorno.Codigo = 1020;
                    _InsertExcelResult.Retorno.Mensagem = "Extensão do arquivo deve ser .xls/.xlsx";
                    return _InsertExcelResult;
                }
                #endregion

                #region Importação
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var stream = new MemoryStream())
                {
                    arquivo.CopyTo(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        long cont = 0;
                        List<ExcelList> oExcelList = new List<ExcelList>();

                        while (reader.Read())
                        {
                            if (cont > 0)
                            {
                                #region Validações dos registros

                                //Linha com algum campo nulo
                                if (reader.GetValue(0) == null || reader.GetValue(1) == null || reader.GetValue(2) == null || reader.GetValue(3) == null)
                                {
                                    _InsertExcelResult.Retorno.Codigo = 1100;
                                    _InsertExcelResult.Retorno.Mensagem = $"Não foi possível importar planilha. Existem campos vazios. Linha {cont}";
                                    return _InsertExcelResult;
                                }

                                //DataEntrega
                                DateTime.TryParse(reader.GetValue(0).ToString(), out DateTime dataEntrega);

                                if (dataEntrega == default)
                                {
                                    _InsertExcelResult.Retorno.Codigo = 1110;
                                    _InsertExcelResult.Retorno.Mensagem = $"Não foi possível importar planilha. Campo Data de Entrega está com formato inválido. Linha {cont}";
                                    return _InsertExcelResult;
                                }

                                if (dataEntrega <= DateTime.Now)
                                {
                                    _InsertExcelResult.Retorno.Codigo = 1110;
                                    _InsertExcelResult.Retorno.Mensagem = $"Não foi possível importar planilha. Data de Entrega não pode ser menor que a data atual. Linha {cont}";
                                    return _InsertExcelResult;
                                }

                                //NomeProduto
                                string nomeProduto = reader.GetValue(1).ToString();

                                if (nomeProduto.Length > 50)
                                {
                                    _InsertExcelResult.Retorno.Codigo = 1120;
                                    _InsertExcelResult.Retorno.Mensagem = $"Não foi possível importar planilha. Nome do Produto excede o tamanho de 50 caracteres. Linha {cont}";
                                    return _InsertExcelResult;
                                }

                                //Quantidade
                                int.TryParse(reader.GetValue(2).ToString(), out int quantidade);

                                if (quantidade == 0)
                                {
                                    _InsertExcelResult.Retorno.Codigo = 1130;
                                    _InsertExcelResult.Retorno.Mensagem = $"Não foi possível importar planilha. Quantidade não pode ser zero. Linha {cont}";
                                    return _InsertExcelResult;
                                }

                                //ValorUnitario
                                decimal.TryParse(reader.GetValue(3).ToString(), out decimal valorUnitario);

                                if (valorUnitario == 0)
                                {
                                    _InsertExcelResult.Retorno.Codigo = 1130;
                                    _InsertExcelResult.Retorno.Mensagem = $"Não foi possível importar planilha. Valor Unitário não pode ser zero. Linha {cont}";
                                    return _InsertExcelResult;
                                }
                                #endregion

                                ExcelList _ExcelList = new ExcelList();

                                _ExcelList.DataEntrega = dataEntrega;
                                _ExcelList.NomeProduto = nomeProduto;
                                _ExcelList.Quantidade = quantidade;
                                _ExcelList.ValorUnitario = Math.Round(valorUnitario, 2);

                                oExcelList.Add(_ExcelList);
                            }

                            cont++;
                        }

                        using (var transaction = _unitOfWork.BeginTransaction())
                        {
                            try
                            {
                                _IMPORTACAO_EXCEL.DataImportacao = DateTime.Now;
                                _IMPORTACAO_EXCEL.NomeArquivo = arquivo.FileName;

                                _unitOfWork.Repository<IMPORTACAO_EXCEL>().Add(_IMPORTACAO_EXCEL);
                                await _unitOfWork.SaveChanges();

                                oEXCEL = _mapper.Map<List<EXCEL>>(oExcelList);
                                oEXCEL.Select(x => { x.Id = _IMPORTACAO_EXCEL.Id; return x; }).ToList();

                                _unitOfWork.Repository<EXCEL>().AddRange(oEXCEL);
                                await _unitOfWork.SaveChanges();

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();

                                _InsertExcelResult.Retorno.Codigo = 1100;
                                _InsertExcelResult.Retorno.Mensagem = $"Insert({arquivo.FileName})): {ex.Message}";
                                return _InsertExcelResult;
                            }
                        }

                        #region Repassa as informações para o retorno
                        _InsertExcelResult.Id = _IMPORTACAO_EXCEL.Id;
                        _InsertExcelResult.NomeArquivo = arquivo.FileName;
                        _InsertExcelResult.DataImportacao = DateTime.Now;
                        _InsertExcelResult.TotalRegistros = cont - 1;
                        _InsertExcelResult.Retorno.Codigo = 200;
                        _InsertExcelResult.Retorno.Mensagem = "Excel importado com sucesso";
                        _InsertExcelResult.Retorno.DataProcessamento = DateTime.Now;
                        #endregion
                    }
                }
                #endregion

                return _InsertExcelResult;
            }
            catch (Exception ex)
            {
                _InsertExcelResult.Retorno.Codigo = 9999;
                _InsertExcelResult.Retorno.Mensagem = $"Insert({arquivo.FileName})): {ex.Message}";
                return _InsertExcelResult;
            }
        }
    }
}