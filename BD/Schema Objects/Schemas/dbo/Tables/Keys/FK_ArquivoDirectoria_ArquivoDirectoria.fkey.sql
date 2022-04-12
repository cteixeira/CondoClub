ALTER TABLE [dbo].[ArquivoDirectoria]
    ADD CONSTRAINT [FK_ArquivoDirectoria_ArquivoDirectoria] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

