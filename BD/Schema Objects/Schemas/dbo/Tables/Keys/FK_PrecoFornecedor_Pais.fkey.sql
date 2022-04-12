ALTER TABLE [dbo].[PrecoFornecedor]
    ADD CONSTRAINT [FK_PrecoFornecedor_Pais] FOREIGN KEY ([PaisID]) REFERENCES [dbo].[Pais] ([PaisID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

