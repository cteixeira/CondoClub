CREATE TABLE [dbo].[MensagemDestinatario] (
    [MensagemDestinatarioID] BIGINT IDENTITY (1, 1) NOT NULL,
    [MensagemID]             BIGINT NOT NULL,
    [DestinatarioID]         BIGINT CONSTRAINT [DF_MensagemDestinatario_DestinatarioID] DEFAULT ((0)) NOT NULL,
    [Visto]                  BIT    NOT NULL,
    CONSTRAINT [PK_MensagemDestinatario] PRIMARY KEY CLUSTERED ([MensagemDestinatarioID] ASC),
    CONSTRAINT [FK_MensagemDestinatario_MensagemDestinatario] FOREIGN KEY ([MensagemID]) REFERENCES [dbo].[Mensagem] ([MensagemID]) ON DELETE CASCADE,
    CONSTRAINT [FK_MensagemDestinatario_Utilizador] FOREIGN KEY ([DestinatarioID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID])
);



