CREATE TABLE [dbo].[Fornecedor] (
    [FornecedorID]       BIGINT          IDENTITY (1, 1) NOT NULL,
    [Nome]               NVARCHAR (200)  COLLATE Latin1_General_CI_AI NOT NULL,
    [Contribuinte]       NVARCHAR (20)   COLLATE Latin1_General_CI_AI NOT NULL,
    [OpcaoPagamentoID]   INT             NOT NULL,
    [FormaPagamentoID]   INT             NOT NULL,
    [AvatarID]           BIGINT          NULL,
    [Descricao]          NVARCHAR (4000) COLLATE Latin1_General_CI_AI NOT NULL,
    [Telefone]           NVARCHAR (20)   NOT NULL,
    [Email]              NVARCHAR (200)  NOT NULL,
    [URL]                NVARCHAR (200)  NULL,
    [Endereco]           NVARCHAR (400)  NOT NULL,
    [Localidade]         NVARCHAR (80)   NULL,
    [Cidade]             NVARCHAR (80)   COLLATE Latin1_General_CI_AI NOT NULL,
    [CodigoPostal]       NVARCHAR (20)   NOT NULL,
    [Estado]             NVARCHAR (80)   COLLATE Latin1_General_CI_AI NULL,
    [PaisID]             INT             NOT NULL,
    [Latitude]           FLOAT (53)      NOT NULL,
    [Longitude]          FLOAT (53)      NOT NULL,
    [RaioAccao]          INT             NOT NULL,
    [Activo]             BIT             CONSTRAINT [DF_Fornecedor_Activo] DEFAULT ((0)) NOT NULL,
    [DataActivacao]      SMALLDATETIME   NULL,
    [ClassificacaoMedia] SMALLINT        NOT NULL,
    [TotalComentarios]   INT             NOT NULL,
    [DataCriacao]        SMALLDATETIME   CONSTRAINT [DF_Fornecedor_DataCriacao] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Fornecedor] PRIMARY KEY CLUSTERED ([FornecedorID] ASC),
    CONSTRAINT [FK_Fornecedor_Ficheiro] FOREIGN KEY ([AvatarID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]),
    CONSTRAINT [FK_Fornecedor_FormaPagamento] FOREIGN KEY ([FormaPagamentoID]) REFERENCES [dbo].[FormaPagamento] ([FormaPagamentoID]),
    CONSTRAINT [FK_Fornecedor_OpcaoPagamento] FOREIGN KEY ([OpcaoPagamentoID]) REFERENCES [dbo].[OpcaoPagamento] ([OpcaoPagamentoID]),
    CONSTRAINT [FK_Fornecedor_Pais] FOREIGN KEY ([PaisID]) REFERENCES [dbo].[Pais] ([PaisID])
);




















GO
CREATE NONCLUSTERED INDEX [IX_Fornecedor_Pesquisa]
    ON [dbo].[Fornecedor]([FornecedorID] ASC, [Activo] ASC, [Nome] ASC, [Contribuinte] ASC, [Cidade] ASC, [Estado] ASC);

