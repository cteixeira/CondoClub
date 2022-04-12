ALTER TABLE [dbo].[FornecedorKeyword]
    ADD CONSTRAINT [FK_FornecedorKeyword_Fornecedor] FOREIGN KEY ([FornecedorID]) REFERENCES [dbo].[Fornecedor] ([FornecedorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

