ALTER TABLE [dbo].[ArquivoFicheiro]
    ADD CONSTRAINT [FK_ArquivoFicheiro_Utilizador] FOREIGN KEY ([UtilizadorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

