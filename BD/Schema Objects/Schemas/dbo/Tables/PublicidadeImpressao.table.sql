CREATE TABLE [dbo].[PublicidadeImpressao] (
    [PublicidadeImpressaoID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [PublicidadeID]          BIGINT        NOT NULL,
    [CondominioID]           BIGINT        NOT NULL,
    [DataHora]               SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_PublicidadeImpressao_1] PRIMARY KEY CLUSTERED ([PublicidadeImpressaoID] ASC),
    CONSTRAINT [FK_PublicidadeImpressao_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]),
    CONSTRAINT [FK_PublicidadeImpressao_Publicidade] FOREIGN KEY ([PublicidadeID]) REFERENCES [dbo].[Publicidade] ([PublicidadeID])
);







