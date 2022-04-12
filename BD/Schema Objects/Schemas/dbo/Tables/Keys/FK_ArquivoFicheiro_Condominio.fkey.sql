ALTER TABLE [dbo].[ArquivoFicheiro]
    ADD CONSTRAINT [FK_ArquivoFicheiro_Condominio] FOREIGN KEY ([CondominioID]) REFERENCES [dbo].[Condominio] ([CondominioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

