ALTER TABLE [dbo].[FornecedorCategoria]
    ADD CONSTRAINT [FK_FornecedorCategoria_FornecedorCategoria1] FOREIGN KEY ([CategoriaID]) REFERENCES [dbo].[Categoria] ([CategoriaID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

