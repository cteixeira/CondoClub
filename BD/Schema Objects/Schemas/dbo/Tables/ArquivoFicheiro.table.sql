CREATE TABLE [dbo].[ArquivoFicheiro] (
    [ArquivoFicheiroID]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CondominioID]        BIGINT         NOT NULL,
    [ArquivoDirectoriaID] BIGINT         NULL,
    [FicheiroID]          BIGINT         NOT NULL,
    [Comentario]          NVARCHAR (400) NULL,
    [DataHora]            SMALLDATETIME  NOT NULL,
    [UtilizadorID]        BIGINT         NOT NULL
);

