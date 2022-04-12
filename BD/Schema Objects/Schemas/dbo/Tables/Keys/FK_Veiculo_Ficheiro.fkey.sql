ALTER TABLE [dbo].[Veiculo]
    ADD CONSTRAINT [FK_Veiculo_Ficheiro] FOREIGN KEY ([FotoID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

