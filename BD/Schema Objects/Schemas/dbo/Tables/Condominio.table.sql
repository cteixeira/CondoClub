CREATE TABLE [dbo].[Condominio] (
    [CondominioID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [EmpresaID]        BIGINT         NULL,
    [Nome]             NVARCHAR (200) COLLATE Latin1_General_CI_AI NOT NULL,
    [Contribuinte]     NVARCHAR (20)  COLLATE Latin1_General_CI_AI NOT NULL,
    [OpcaoPagamentoID] INT            NOT NULL,
    [FormaPagamentoID] INT            NOT NULL,
    [Fraccoes]         SMALLINT       NULL,
    [ExtratoSocialID]  INT            NULL,
    [AvatarID]         BIGINT         NULL,
    [Endereco]         NVARCHAR (400) NOT NULL,
    [Localidade]       NVARCHAR (80)  COLLATE Latin1_General_CI_AI NULL,
    [Cidade]           NVARCHAR (80)  COLLATE Latin1_General_CI_AI NOT NULL,
    [CodigoPostal]     NVARCHAR (20)  NOT NULL,
    [Estado]           NVARCHAR (80)  COLLATE Latin1_General_CI_AI NULL,
    [PaisID]           INT            NOT NULL,
    [Latitude]         FLOAT (53)     NOT NULL,
    [Longitude]        FLOAT (53)     NOT NULL,
    [Activo]           BIT            CONSTRAINT [DF_Condominio_Activo] DEFAULT ((0)) NOT NULL,
    [DataActivacao]    SMALLDATETIME  NULL,
    [DataCriacao]      SMALLDATETIME  CONSTRAINT [DF_Condominio_DataCriacao] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Condominio] PRIMARY KEY CLUSTERED ([CondominioID] ASC),
    CONSTRAINT [FK_Condominio_Empresa] FOREIGN KEY ([EmpresaID]) REFERENCES [dbo].[Empresa] ([EmpresaID]),
    CONSTRAINT [FK_Condominio_ExtratoSocial] FOREIGN KEY ([ExtratoSocialID]) REFERENCES [dbo].[ExtratoSocial] ([ExtratoSocialID]),
    CONSTRAINT [FK_Condominio_Ficheiro] FOREIGN KEY ([AvatarID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]),
    CONSTRAINT [FK_Condominio_FormaPagamento] FOREIGN KEY ([FormaPagamentoID]) REFERENCES [dbo].[FormaPagamento] ([FormaPagamentoID]),
    CONSTRAINT [FK_Condominio_OpcaoPagamento] FOREIGN KEY ([OpcaoPagamentoID]) REFERENCES [dbo].[OpcaoPagamento] ([OpcaoPagamentoID]),
    CONSTRAINT [FK_Condominio_Pais] FOREIGN KEY ([PaisID]) REFERENCES [dbo].[Pais] ([PaisID])
);













