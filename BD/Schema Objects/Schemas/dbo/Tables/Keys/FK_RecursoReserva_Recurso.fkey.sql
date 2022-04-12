ALTER TABLE [dbo].[RecursoReserva]
    ADD CONSTRAINT [FK_RecursoReserva_Recurso] FOREIGN KEY ([RecursoID]) REFERENCES [dbo].[Recurso] ([RecursoID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

