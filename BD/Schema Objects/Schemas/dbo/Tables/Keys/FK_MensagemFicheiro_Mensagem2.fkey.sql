ALTER TABLE [dbo].[MensagemFicheiro]
    ADD CONSTRAINT [FK_MensagemFicheiro_Mensagem2] FOREIGN KEY ([MensagemID]) REFERENCES [dbo].[Mensagem] ([MensagemID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

