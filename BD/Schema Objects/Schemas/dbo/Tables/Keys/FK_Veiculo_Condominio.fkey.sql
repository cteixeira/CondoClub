ALTER TABLE [dbo].[Veiculo]
    ADD CONSTRAINT [FK_Veiculo_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

