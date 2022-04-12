ALTER TABLE [dbo].[Recurso]
    ADD CONSTRAINT [FK_Recurso_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

