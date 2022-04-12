ALTER TABLE [dbo].[Funcionario]
    ADD CONSTRAINT [FK_Funcionario_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

