CREATE TABLE [dbo].[FornecedorClassificacao] (
    [FornecedorClassificacaoID] BIGINT          IDENTITY (1, 1) NOT NULL,
    [FornecedorID]              BIGINT          NOT NULL,
    [UtilizadorID]              BIGINT          NOT NULL,
    [Classificacao]             SMALLINT        NOT NULL,
    [Comentario]                NVARCHAR (2000) NULL,
    [DataHora]                  SMALLDATETIME   NOT NULL
);









