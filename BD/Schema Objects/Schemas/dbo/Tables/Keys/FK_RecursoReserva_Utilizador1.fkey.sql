ALTER TABLE [dbo].[RecursoReserva]
    ADD CONSTRAINT [FK_RecursoReserva_Utilizador1] FOREIGN KEY ([UtilizadorAprovacaoID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

