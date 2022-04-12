CREATE TABLE [dbo].[LogTarefa] (
    [LogTarefaID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [DataHora]    SMALLDATETIME  NOT NULL,
    [Descricao]   NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_LogTarefa] PRIMARY KEY CLUSTERED ([LogTarefaID] ASC)
);

