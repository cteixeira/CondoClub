ALTER TABLE [dbo].[Agenda]
    ADD CONSTRAINT [FK_Agenda_Agenda] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

