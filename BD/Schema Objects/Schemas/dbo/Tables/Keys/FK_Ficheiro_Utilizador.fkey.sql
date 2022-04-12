ALTER TABLE [dbo].[Ficheiro]
    ADD CONSTRAINT [FK_Ficheiro_Utilizador] FOREIGN KEY ([UtilizadorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

