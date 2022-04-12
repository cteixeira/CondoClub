ALTER TABLE [dbo].[Questionario]
    ADD CONSTRAINT [FK_Questionario_Autor] FOREIGN KEY ([AutorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

