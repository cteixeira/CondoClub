ALTER TABLE [dbo].[FornecedorClassificacao]
    ADD CONSTRAINT [FK_FornecedorClassificacao_Utilizador] FOREIGN KEY ([UtilizadorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

