CREATE TABLE [dbo].[QuestionarioResposta] (
    [QuestionarioRespostaID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [QuestionarioID]         BIGINT         NOT NULL,
    [MoradorID]              BIGINT         NOT NULL,
    [OpcaoSeleccionada]      SMALLINT       NOT NULL,
    [OutraOpcao]             NVARCHAR (800) NULL,
    [DataHora]               SMALLDATETIME  NOT NULL
);



