using System;
using System.Collections.Generic;
using System.Text;

namespace CondoClub.Regras.Exceptions {

    public class Autenticacao : Exception { }

    public class RegistoPorAprovar : Exception { }

    public class UserNameRepetido : Exception { }

    public class EmailRepetido : Exception { }

    public class DadosIncorrectos : Exception { }

    public class SemPermissao : Exception { }

    public class TemDependencias : Exception { }

    public class FormatoImagemInvalido : Exception { }

    public class DesignacaoRepetida : Exception { }

    public class CodigoRepetido : Exception { }

    public class FornecedorNaoExiste : Exception { }

    public class NomeRepetido : Exception { }

    public class MatriculaRepetida : Exception { }

    public class PrecoPorDefinir : Exception { }

    public class RecursoNaoExiste : Exception { }

    public class RecursoInactivo : Exception { }

    public class CondominioSemSindico : Exception { }

    public class RecursoComReservas : Exception { }

    public class CategoriaRepetida : Exception { }

    public class IntegracaoCielo : Exception {
        public IntegracaoCielo(string message) : base(message) {

        }
    }

    public class FraccaoObrigatoria : Exception { }

    public class PrecoRepetidoRange : Exception { }

    public class EmpresaSemUtilizador : Exception { }

    public class FornecedorSemUtilizador : Exception { }

}
