ALTER TABLE [dbo].[Veiculo]
    ADD CONSTRAINT [FK_Veiculo_Utilizador] FOREIGN KEY ([MoradorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

