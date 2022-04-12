CREATE TABLE [dbo].[RecursoHorario] (
    [RecursoHorarioID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [RecursoID]        BIGINT        NOT NULL,
    [DiaSemana]        SMALLINT      NOT NULL,
    [Inicio]           SMALLDATETIME NOT NULL,
    [NumeroSlots]      SMALLINT      NOT NULL,
    [DuracaoSlot]      SMALLINT      NOT NULL
);



