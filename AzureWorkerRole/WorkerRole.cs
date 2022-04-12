using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace CondoClub.AzureWorkerRole {

    public class WorkerRole : RoleEntryPoint {

        public override void Run() {

            while (true) {

                if (DateTime.Now.TimeOfDay > new TimeSpan(1, 0, 0)) {
                    //Adormece durante 30 minutos
                    Regras.Util.LogTarefa("Sleeping");
                    Thread.Sleep(1800000);
                    continue;
                }
                
                Regras.UtilizadorAutenticado utilizador = null;

                try {
                    utilizador = new Regras.Utilizador()
                        .AbrirUtilizadorAutenticado(Convert.ToInt64(ConfigurationManager.AppSettings["UtilizadorTarefas"]));

                    if (utilizador != null) {
                        Regras.Pagamento pagamento = new Regras.Pagamento();
                        Regras.Util.LogTarefa("Inicio processamento Pagamento");
                        pagamento.ProcessaTodosPagamentos(utilizador);
                        Regras.Util.LogTarefa("Inicio processamento Atraso Pagamento");
                        pagamento.ProcessaAtrasoPagamentos(utilizador);

                        Regras.Fornecedor fornecedor = new Regras.Fornecedor();
                        Regras.Util.LogTarefa("Inicio processamento Alcance Fornecedor");
                        fornecedor.DefineAlcanceFornecedor(null, null, utilizador);
                        Regras.Util.LogTarefa("Inicio processamento Keyword Fornecedor");
                        fornecedor.DefineKeywordsFornecedor(null, utilizador);
                    }

                }
                catch (Exception ex) {
                    Regras.Util.LogTarefa("Erro de processamento: " + ex.Message);
                    Regras.Util.TratamentoErro(null, "Tarefas", ex, utilizador);
                }
                finally {
                    //Adormece durante 120 minutos
                    Regras.Util.LogTarefa("Fim de processamento");
                    Thread.Sleep(7200000);
                }
            }
        }

        public override bool OnStart() {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            return base.OnStart();
        }
    }
}
