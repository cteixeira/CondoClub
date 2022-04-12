ALTER TABLE [dbo].[Publicidade]
    ADD CONSTRAINT [FK_Publicidade_FormaPagamento] FOREIGN KEY ([FormaPagamentoID]) REFERENCES [dbo].[FormaPagamento] ([FormaPagamentoID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

