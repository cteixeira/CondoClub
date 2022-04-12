CREATE TABLE [dbo].[ArquivoDirectoria] (
    [ArquivoDirectoriaID]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [CondominioID]           BIGINT         NOT NULL,
    [ArquivoDirectoriaPaiID] BIGINT         NULL,
    [Nome]                   NVARCHAR (200) NOT NULL,
    [DataHora]               SMALLDATETIME  NOT NULL,
    [UtilizadorID]           BIGINT         NOT NULL
);

