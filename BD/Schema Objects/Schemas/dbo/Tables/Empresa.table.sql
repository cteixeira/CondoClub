CREATE TABLE [dbo].[Empresa] (
    [EmpresaID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [Nome]          NVARCHAR (200) COLLATE Latin1_General_CI_AI NOT NULL,
    [Contribuinte]  NVARCHAR (20)  COLLATE Latin1_General_CI_AI NULL,
    [AvatarID]      BIGINT         NULL,
    [Endereco]      NVARCHAR (400) NOT NULL,
    [Localidade]    NVARCHAR (80)  NULL,
    [Cidade]        NVARCHAR (80)  COLLATE Latin1_General_CI_AI NOT NULL,
    [CodigoPostal]  NVARCHAR (20)  NOT NULL,
    [Estado]        NVARCHAR (80)  COLLATE Latin1_General_CI_AI NULL,
    [PaisID]        INT            NOT NULL,
    [Activo]        BIT            CONSTRAINT [DF_Empresa_Activo] DEFAULT ((0)) NOT NULL,
    [DataActivacao] SMALLDATETIME  NULL,
    [Latitude]      FLOAT (53)     NOT NULL,
    [Longitude]     FLOAT (53)     NOT NULL,
    [DataCriacao]   SMALLDATETIME  CONSTRAINT [DF_Empresa_DataCriacao] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Empresa] PRIMARY KEY CLUSTERED ([EmpresaID] ASC),
    CONSTRAINT [FK_Empresa_Ficheiro] FOREIGN KEY ([AvatarID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]),
    CONSTRAINT [FK_Empresa_Pais] FOREIGN KEY ([PaisID]) REFERENCES [dbo].[Pais] ([PaisID])
);












GO
CREATE NONCLUSTERED INDEX [IX_Empresa_Pesquisa]
    ON [dbo].[Empresa]([EmpresaID] ASC, [Activo] ASC, [Nome] ASC, [Contribuinte] ASC, [Cidade] ASC, [Estado] ASC);

