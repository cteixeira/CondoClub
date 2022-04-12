ALTER TABLE [dbo].[Localizacao]
    ADD CONSTRAINT [FK_Localizacao_TabelaLocalizada] FOREIGN KEY ([TabelaLocalizadaID]) REFERENCES [dbo].[TabelaLocalizada] ([TabelaLocalizadaID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

