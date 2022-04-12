ALTER TABLE [dbo].[Publicidade]
    ADD CONSTRAINT [FK_Publicidade_Utilizador2] FOREIGN KEY ([UtilizadorPagamentoID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

