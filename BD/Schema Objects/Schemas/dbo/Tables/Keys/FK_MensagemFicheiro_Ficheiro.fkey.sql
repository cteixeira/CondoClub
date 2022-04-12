ALTER TABLE [dbo].[MensagemFicheiro]
    ADD CONSTRAINT [FK_MensagemFicheiro_Ficheiro] FOREIGN KEY ([FicheiroID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

