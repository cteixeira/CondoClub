ALTER TABLE [dbo].[Comunicado]
    ADD CONSTRAINT [FK_Comunicado_Ficheiro] FOREIGN KEY ([ImagemID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

