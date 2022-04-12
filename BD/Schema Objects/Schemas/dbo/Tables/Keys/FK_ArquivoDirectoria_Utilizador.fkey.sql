ALTER TABLE [dbo].[ArquivoDirectoria]
    ADD CONSTRAINT [FK_ArquivoDirectoria_Utilizador] FOREIGN KEY ([UtilizadorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

