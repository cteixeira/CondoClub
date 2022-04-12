CREATE TABLE [dbo].[PrecoPublicidade] (
    [PrecoPublicidadeID] INT             IDENTITY (1, 1) NOT NULL,
    [PaisID]             INT             NOT NULL,
    [ZonaPublicidadeID]  INT             NOT NULL,
    [FraccoesAte]        BIGINT          NOT NULL,
    [Valor]              DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_PrecoPublicidade] PRIMARY KEY CLUSTERED ([PrecoPublicidadeID] ASC),
    CONSTRAINT [FK_PrecoPublicidade_Pais] FOREIGN KEY ([PaisID]) REFERENCES [dbo].[Pais] ([PaisID]),
    CONSTRAINT [FK_PrecoPublicidade_ZonaPublicidade] FOREIGN KEY ([ZonaPublicidadeID]) REFERENCES [dbo].[ZonaPublicidade] ([ZonaPublicidadeID])
);





