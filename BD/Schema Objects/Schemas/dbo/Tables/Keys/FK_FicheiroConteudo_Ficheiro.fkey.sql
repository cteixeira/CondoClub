ALTER TABLE [dbo].[FicheiroConteudo]
    ADD CONSTRAINT [FK_FicheiroConteudo_Ficheiro] FOREIGN KEY ([FicheiroID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]) ON DELETE CASCADE ON UPDATE NO ACTION;

