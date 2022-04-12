using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Tarefas {
    
    class Program {

        static void Main(string[] args) {

            Regras.UtilizadorAutenticado utilizador = null;

            try {
                utilizador = new Regras.Utilizador()
                    .AbrirUtilizadorAutenticado(Convert.ToInt64(ConfigurationManager.AppSettings["UtilizadorTarefas"]));

                if (utilizador != null) {
                    Regras.Pagamento pagamento = new Regras.Pagamento();
                    pagamento.ProcessaTodosPagamentos(utilizador);
                    pagamento.ProcessaAtrasoPagamentos(utilizador);

                    Regras.Fornecedor fornecedor = new Regras.Fornecedor();
                    fornecedor.DefineAlcanceFornecedor(null, null, utilizador);
                    fornecedor.DefineKeywordsFornecedor(null, utilizador);
                }

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, "Tarefas", ex, utilizador);
            }
        }

    }
}
