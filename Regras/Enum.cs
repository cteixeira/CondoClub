using System;
using System.Collections.Generic;
using System.Text;

namespace CondoClub.Regras.Enum {
    
    public enum Pais {
        Brasil = 1,
        Portugal
    }

    public enum TabelaLocalizada {
        ExtratoSocial = 1,
        PerfilUtilizador,
        ZonaPublicidade,
        FormaPagamento,
        OpcaoPagamento
    }

    public enum Perfil {
        CondoClub = 1,
        Empresa,
        Síndico,
        Morador,
        Portaria,
        Consulta,
        Fornecedor
    }

    public enum Permissao {
        Visualizar,
        Gravar,
        Apagar
    }

    public enum ExtratoSocial {
        Baixa = 1,
        Media,
        Alta
    }

    public enum OpcaoPagamento {
        Mensal = 1,
        Anual
    }

    public enum FormaPagamento {
        CartaoCredito = 1,
        Boleto
    }

    public enum ZonaPublicidade {
        Topo = 1,
        Lateral
    }

    public enum DimensaoFicheiro {
        Indiferente,
        ImagemComunicado,
        ImagemFormulario,
        ImagemPublicidade
    }

    public enum EstadoRecursoReserva {
        Reservado,
        PendenteAprovacao,
        NaoAprovado
    }

    public enum OrigemPagamento {
        Condominio,
        Fornecedor,
        Publicidade
    }

    public enum TipoCartaoCredito {
        Visa,
        MasterCard,
        Diners,
        Discover,
        Elo,
        Amex,
        Jcb,
        Aura
    }

    public enum TipoEstatistica {
        NovoComunicado,
        NovaMensagem,
        NovoQuestionario,
        NovaReserva,
        NovoArquivo,
        NovoMorador,
        NovoCondominio,
        NovoFornecedor,
        NovaAdministradora,
        NovaPublicidade,
        ImpressaoPublicidade,
        CliquePublicidade,
        PagamentoPublicidade,
        PagamentoFornecedor,
        PagamentoCondominio,
        Erro
    }

}
