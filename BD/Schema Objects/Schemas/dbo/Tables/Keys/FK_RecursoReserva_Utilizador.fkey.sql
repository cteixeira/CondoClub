ALTER TABLE [dbo].[RecursoReserva]
    ADD CONSTRAINT [FK_RecursoReserva_Utilizador] FOREIGN KEY ([MoradorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

