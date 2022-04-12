ALTER TABLE [dbo].[Comunicado]
    ADD CONSTRAINT [FK_Comunicado_Utilizador] FOREIGN KEY ([RemetenteID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

