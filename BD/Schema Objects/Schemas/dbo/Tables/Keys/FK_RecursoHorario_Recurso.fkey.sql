ALTER TABLE [dbo].[RecursoHorario]
    ADD CONSTRAINT [FK_RecursoHorario_Recurso] FOREIGN KEY ([RecursoID]) REFERENCES [dbo].[Recurso] ([RecursoID]) ON DELETE CASCADE ON UPDATE NO ACTION;



