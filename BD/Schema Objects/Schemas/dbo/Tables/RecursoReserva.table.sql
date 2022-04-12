CREATE TABLE [dbo].[RecursoReserva] (
    [RecursoReservaID]      BIGINT          IDENTITY (1, 1) NOT NULL,
    [RecursoID]             BIGINT          NOT NULL,
    [MoradorID]             BIGINT          NOT NULL,
    [DataHoraInicio]        SMALLDATETIME   NOT NULL,
    [NumeroSlots]           SMALLINT        NOT NULL,
    [Comentario]            NVARCHAR (4000) NULL,
    [Aprovado]              BIT             NULL,
    [DataAprovacao]         SMALLDATETIME   NULL,
    [UtilizadorAprovacaoID] BIGINT          NULL
);

