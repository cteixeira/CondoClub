ALTER TABLE [dbo].[QuestionarioResposta]
    ADD CONSTRAINT [FK_QuestionarioResposta_Utilizador] FOREIGN KEY ([MoradorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

