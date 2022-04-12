ALTER TABLE [dbo].[Comunicado]
    ADD CONSTRAINT [FK_Comunicado_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

