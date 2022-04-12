CREATE TABLE [dbo].[Ficheiro] (
    [FicheiroID]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Nome]         NVARCHAR (200) NOT NULL,
    [Extensao]     NVARCHAR (5)   NOT NULL,
    [DataHora]     SMALLDATETIME  NOT NULL,
    [UtilizadorID] BIGINT         NULL,
    [Temporario]   BIT            NOT NULL,
    [Tamanho]      INT            NULL
);





















