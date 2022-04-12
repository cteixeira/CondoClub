CREATE TABLE [dbo].[Localizacao] (
    [LocalizacaoID]      INT            IDENTITY (1, 1) NOT NULL,
    [TabelaLocalizadaID] INT            NOT NULL,
    [Identificador]      INT            NOT NULL,
    [PaisID]             INT            NOT NULL,
    [Designacao]         NVARCHAR (200) NOT NULL
);

