ALTER TABLE [dbo].[PrecoCondominio]
    ADD CONSTRAINT [FK_PrecoCondominio_OpcaoPagamento] FOREIGN KEY ([OpcaoPagamentoID]) REFERENCES [dbo].[OpcaoPagamento] ([OpcaoPagamentoID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

