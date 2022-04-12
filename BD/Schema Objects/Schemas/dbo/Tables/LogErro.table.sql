CREATE TABLE [dbo].[LogErro] (
    [LogErroID]    BIGINT          IDENTITY (1, 1) NOT NULL,
    [Importancia]  SMALLINT        NOT NULL,
    [Origem]       NVARCHAR (200)  NOT NULL,
    [Descricao]    NVARCHAR (4000) NOT NULL,
    [DataHora]     DATETIME        NOT NULL,
    [UtilizadorID] BIGINT          NULL
);

