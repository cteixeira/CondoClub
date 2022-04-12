CREATE TABLE [dbo].[CondominioPagamento] (
    [CondominioPagamentoID] BIGINT          IDENTITY (1, 1) NOT NULL,
    [CondominioID]          BIGINT          NOT NULL,
    [Inicio]                SMALLDATETIME   NOT NULL,
    [Fim]                   SMALLDATETIME   NOT NULL,
    [Valor]                 DECIMAL (18, 2) NOT NULL,
    [Pago]                  BIT             CONSTRAINT [DF_CondominioPagamento_Pago] DEFAULT ((0)) NOT NULL,
    [DataEmissao]           SMALLDATETIME   NOT NULL,
    [FormaPagamentoID]      INT             NOT NULL,
    [ReferenciaPagamento]   NVARCHAR (200)  NULL,
    [DataPagamento]         SMALLDATETIME   NULL,
    [UtilizadorPagamentoID] BIGINT          NULL,
    [IdTransacaoCielo]      NVARCHAR (50)   NULL,
    [IdTransacaoBoleto]     NVARCHAR (100)  NULL,
    CONSTRAINT [PK_CondominioPagamento] PRIMARY KEY CLUSTERED ([CondominioPagamentoID] ASC),
    CONSTRAINT [FK_CondominioPagamento_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]),
    CONSTRAINT [FK_CondominioPagamento_FormaPagamento] FOREIGN KEY ([FormaPagamentoID]) REFERENCES [dbo].[FormaPagamento] ([FormaPagamentoID]),
    CONSTRAINT [FK_CondominioPagamento_Utilizador] FOREIGN KEY ([UtilizadorPagamentoID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID])
);





