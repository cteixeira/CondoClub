ALTER TABLE [dbo].[FornecedorAlcance]
    ADD CONSTRAINT [FK_FornecedorAlcance_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

