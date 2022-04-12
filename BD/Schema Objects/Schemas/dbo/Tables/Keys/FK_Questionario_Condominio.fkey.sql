ALTER TABLE [dbo].[Questionario]
    ADD CONSTRAINT [FK_Questionario_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

