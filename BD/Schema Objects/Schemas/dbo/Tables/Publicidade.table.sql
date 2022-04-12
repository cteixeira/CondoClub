CREATE TABLE [dbo].[Publicidade] (
    [PublicidadeID]         BIGINT          IDENTITY (1, 1) NOT NULL,
    [FornecedorID]          BIGINT          NOT NULL,
    [ZonaPublicidadeID]     INT             NOT NULL,
    [Titulo]                NVARCHAR (200)  COLLATE Latin1_General_CI_AI NOT NULL,
    [Texto]                 NVARCHAR (800)  NULL,
    [ImagemID]              BIGINT          NULL,
    [URL]                   NVARCHAR (200)  NULL,
    [RaioAccao]             INT             NOT NULL,
    [AlcanceCondominios]    INT             NOT NULL,
    [AlcanceFraccoes]       BIGINT          NOT NULL,
    [Inicio]                SMALLDATETIME   NOT NULL,
    [Fim]                   SMALLDATETIME   NOT NULL,
    [DataCriacao]           SMALLDATETIME   NOT NULL,
    [UtilizadorCriacaoID]   BIGINT          NOT NULL,
    [Aprovado]              BIT             CONSTRAINT [DF_Publicidade_Aprovada] DEFAULT ((0)) NOT NULL,
    [DataAprovacao]         SMALLDATETIME   NULL,
    [UtilizadorAprovacaoID] BIGINT          NULL,
    [Valor]                 DECIMAL (18, 2) NULL,
    [Pago]                  BIT             CONSTRAINT [DF_Publicidade_Pago] DEFAULT ((0)) NOT NULL,
    [DataEmissao]           SMALLDATETIME   NULL,
    [FormaPagamentoID]      INT             NULL,
    [ReferenciaPagamento]   NVARCHAR (200)  NULL,
    [DataPagamento]         SMALLDATETIME   NULL,
    [UtilizadorPagamentoID] BIGINT          NULL,
    [IdTransacaoCielo]      NVARCHAR (50)   NULL,
    [IdTransacaoBoleto]     NVARCHAR (100)  NULL,
    CONSTRAINT [PK_Publicidade] PRIMARY KEY CLUSTERED ([PublicidadeID] ASC),
    CONSTRAINT [FK_Publicidade_Ficheiro] FOREIGN KEY ([ImagemID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]),
    CONSTRAINT [FK_Publicidade_FormaPagamento] FOREIGN KEY ([FormaPagamentoID]) REFERENCES [dbo].[FormaPagamento] ([FormaPagamentoID]),
    CONSTRAINT [FK_Publicidade_Fornecedor] FOREIGN KEY ([FornecedorID]) REFERENCES [dbo].[Fornecedor] ([FornecedorID]),
    CONSTRAINT [FK_Publicidade_Utilizador] FOREIGN KEY ([UtilizadorCriacaoID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]),
    CONSTRAINT [FK_Publicidade_Utilizador1] FOREIGN KEY ([UtilizadorAprovacaoID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]),
    CONSTRAINT [FK_Publicidade_Utilizador2] FOREIGN KEY ([UtilizadorPagamentoID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]),
    CONSTRAINT [FK_Publicidade_ZonaPublicidade] FOREIGN KEY ([ZonaPublicidadeID]) REFERENCES [dbo].[ZonaPublicidade] ([ZonaPublicidadeID])
);
















GO
CREATE NONCLUSTERED INDEX [IX_Titulo]
    ON [dbo].[Publicidade]([Titulo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Fornecedor]
    ON [dbo].[Publicidade]([FornecedorID] ASC);

