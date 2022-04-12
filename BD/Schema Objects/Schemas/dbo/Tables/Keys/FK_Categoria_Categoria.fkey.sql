ALTER TABLE [dbo].[Categoria]
    ADD CONSTRAINT [FK_Categoria_Categoria] FOREIGN KEY ([CategoriaPaiID]) REFERENCES [dbo].[Categoria] ([CategoriaID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

