CREATE TABLE [dbo].[Utilizador] (
    [UtilizadorID]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [PerfilUtilizadorID] INT            NOT NULL,
    [Email]              NVARCHAR (200) NOT NULL,
    [Password]           NVARCHAR (200) NULL,
    [Nome]               NVARCHAR (200) NULL,
    [AvatarID]           BIGINT         NULL,
    [Activo]             BIT            CONSTRAINT [DF_Utilizador_Activo] DEFAULT ((1)) NOT NULL,
    [EmpresaID]          BIGINT         NULL,
    [CondominioID]       BIGINT         NULL,
    [FornecedorID]       BIGINT         NULL,
    [Fraccao]            NVARCHAR (50)  NULL,
    [DataCriacao]        SMALLDATETIME  CONSTRAINT [DF_Utilizador_DataCriacao] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Utilizador] PRIMARY KEY CLUSTERED ([UtilizadorID] ASC),
    CONSTRAINT [FK_Utilizador_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]),
    CONSTRAINT [FK_Utilizador_Empresa] FOREIGN KEY ([EmpresaID]) REFERENCES [dbo].[Empresa] ([EmpresaID]),
    CONSTRAINT [FK_Utilizador_Ficheiro] FOREIGN KEY ([AvatarID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]),
    CONSTRAINT [FK_Utilizador_Fornecedor] FOREIGN KEY ([FornecedorID]) REFERENCES [dbo].[Fornecedor] ([FornecedorID]),
    CONSTRAINT [FK_Utilizador_PerfilUtilizador] FOREIGN KEY ([PerfilUtilizadorID]) REFERENCES [dbo].[PerfilUtilizador] ([PerfilUtilizadorID])
);










GO
CREATE NONCLUSTERED INDEX [IX_Utilizador_Pesquisa]
    ON [dbo].[Utilizador]([Activo] ASC, [PerfilUtilizadorID] ASC, [CondominioID] ASC, [EmpresaID] ASC, [FornecedorID] ASC, [Nome] ASC);

