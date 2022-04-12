ALTER TABLE [dbo].[Mensagem]
    ADD CONSTRAINT [FK_Mensagem_Mensagem] FOREIGN KEY ([RespostaID]) REFERENCES [dbo].[Mensagem] ([MensagemID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

