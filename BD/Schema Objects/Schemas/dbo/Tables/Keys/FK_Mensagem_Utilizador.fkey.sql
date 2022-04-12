ALTER TABLE [dbo].[Mensagem]
    ADD CONSTRAINT [FK_Mensagem_Utilizador] FOREIGN KEY ([RemetenteID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

