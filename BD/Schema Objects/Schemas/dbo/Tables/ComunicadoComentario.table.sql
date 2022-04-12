CREATE TABLE [dbo].[ComunicadoComentario] (
    [ComunicadoComentarioID] BIGINT          IDENTITY (1, 1) NOT NULL,
    [ComunicadoID]           BIGINT          NOT NULL,
    [ComentadorID]           BIGINT          NOT NULL,
    [Gosto]                  BIT             NULL,
    [Comentario]             NVARCHAR (2000) NULL,
    [DataHora]               SMALLDATETIME   NOT NULL
);











