ALTER TABLE [dbo].[FornecedorCategoria]
    ADD CONSTRAINT [FK_FornecedorCategoria_FornecedorCategoria] FOREIGN KEY ([FornecedorID]) REFERENCES [dbo].[Fornecedor] ([FornecedorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

