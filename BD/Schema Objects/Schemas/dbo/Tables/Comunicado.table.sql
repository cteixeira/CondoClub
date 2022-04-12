CREATE TABLE [dbo].[Comunicado] (
    [ComunicadoID] BIGINT          IDENTITY (1, 1) NOT NULL,
    [RemetenteID]  BIGINT          NOT NULL,
    [EmpresaID]    BIGINT          NULL,
    [CondominioID] BIGINT          NULL,
    [Texto]        NVARCHAR (4000) NULL,
    [Video]        NVARCHAR (200)  NULL,
    [ImagemID]     BIGINT          NULL,
    [DataHora]     SMALLDATETIME   NOT NULL
);





