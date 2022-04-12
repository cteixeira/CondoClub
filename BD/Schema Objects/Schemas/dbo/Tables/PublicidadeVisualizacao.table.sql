CREATE TABLE [dbo].[PublicidadeVisualizacao] (
    [PublicidadeVisualizacaoID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [PublicidadeID]             BIGINT        NOT NULL,
    [CondominioID]              BIGINT        NOT NULL,
    [UtilizadorID]              BIGINT        NOT NULL,
    [DataHora]                  SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_PublicidadeVisualizacao] PRIMARY KEY CLUSTERED ([PublicidadeVisualizacaoID] ASC),
    CONSTRAINT [FK_PublicidadeVisualizacao_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]),
    CONSTRAINT [FK_PublicidadeVisualizacao_Publicidade] FOREIGN KEY ([PublicidadeID]) REFERENCES [dbo].[Publicidade] ([PublicidadeID]),
    CONSTRAINT [FK_PublicidadeVisualizacao_Utilizador] FOREIGN KEY ([UtilizadorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID])
);





