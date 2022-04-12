CREATE TABLE [dbo].[FornecedorPagamento] (
    [FornecedorPagamentoID] BIGINT          IDENTITY (1, 1) NOT NULL,
    [FornecedorID]          BIGINT          NOT NULL,
    [Inicio]                SMALLDATETIME   NOT NULL,
    [Fim]                   SMALLDATETIME   NOT NULL,
    [Valor]                 DECIMAL (18, 2) NOT NULL,
    [Pago]                  BIT             CONSTRAINT [DF_FornecedorPagamento_Pago] DEFAULT ((0)) NOT NULL,
    [DataEmissao]           SMALLDATETIME   NOT NULL,
    [FormaPagamentoID]      INT             NOT NULL,
    [ReferenciaPagamento]   NVARCHAR (200)  NULL,
    [DataPagamento]         SMALLDATETIME   NULL,
    [UtilizadorPagamentoID] BIGINT          NULL,
    [IdTransacaoCielo]      NVARCHAR (50)   NULL,
    [IdTransacaoBoleto]     NVARCHAR (100)  NULL,
    CONSTRAINT [PK_FornecedorPagamento] PRIMARY KEY CLUSTERED ([FornecedorPagamentoID] ASC),
    CONSTRAINT [FK_FornecedorPagamento_FormaPagamento] FOREIGN KEY ([FormaPagamentoID]) REFERENCES [dbo].[FormaPagamento] ([FormaPagamentoID]),
    CONSTRAINT [FK_FornecedorPagamento_Fornecedor] FOREIGN KEY ([FornecedorID]) REFERENCES [dbo].[Fornecedor] ([FornecedorID]),
    CONSTRAINT [FK_FornecedorPagamento_Utilizador] FOREIGN KEY ([UtilizadorPagamentoID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID])
);





