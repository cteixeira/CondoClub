ALTER TABLE [dbo].[FornecedorClassificacao]
    ADD CONSTRAINT [FK_FornecedorClassificacao_Fornecedor] FOREIGN KEY ([FornecedorID]) REFERENCES [dbo].[Fornecedor] ([FornecedorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

