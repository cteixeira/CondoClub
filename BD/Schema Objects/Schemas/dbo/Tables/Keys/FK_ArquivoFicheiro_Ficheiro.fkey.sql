ALTER TABLE [dbo].[ArquivoFicheiro]
    ADD CONSTRAINT [FK_ArquivoFicheiro_Ficheiro] FOREIGN KEY ([FicheiroID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

