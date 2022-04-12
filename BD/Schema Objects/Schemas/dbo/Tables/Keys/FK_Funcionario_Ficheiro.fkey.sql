ALTER TABLE [dbo].[Funcionario]
    ADD CONSTRAINT [FK_Funcionario_Ficheiro] FOREIGN KEY ([FotoID]) REFERENCES [dbo].[Ficheiro] ([FicheiroID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

