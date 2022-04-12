CREATE TABLE [dbo].[Mensagem] (
    [MensagemID]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [RemetenteID]             BIGINT          NOT NULL,
    [TextoCabecalho]          NVARCHAR (100)  NULL,
    [Texto]                   NVARCHAR (4000) NOT NULL,
    [DataHora]                DATETIME        NOT NULL,
    [RespostaID]              BIGINT          NULL,
    [UltimaRespostaData]      DATETIME        NULL,
    [UltimaRespostaRemetente] NVARCHAR (100)  NULL,
    [UltimaRespostaTexto]     NVARCHAR (200)  NULL,
    [OrigemNotificacao]       SMALLINT        NULL,
    [OrigemNotificacaoID]     BIGINT          NULL
);













