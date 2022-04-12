ALTER TABLE [dbo].[PrecoFornecedor]
    ADD CONSTRAINT [FK_PrecoFornecedor_OpcaoPagamento] FOREIGN KEY ([OpcaoPagamentoID]) REFERENCES [dbo].[OpcaoPagamento] ([OpcaoPagamentoID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

