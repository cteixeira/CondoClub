CREATE TABLE [dbo].[Recurso] (
    [RecursoID]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [CondominioID]     BIGINT         NOT NULL,
    [Designacao]       NVARCHAR (200) NOT NULL,
    [Activo]           BIT            NOT NULL,
    [RequerAprovacao]  BIT            NOT NULL,
    [DiasMinAprovacao] SMALLINT       NULL,
    [MaxSlotsReserva]  SMALLINT       NOT NULL,
    [IntervaloReserva] SMALLINT       NOT NULL
);





