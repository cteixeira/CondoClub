ALTER TABLE [dbo].[QuestionarioResposta]
    ADD CONSTRAINT [FK_QuestionarioResposta_Questionario] FOREIGN KEY ([QuestionarioID]) REFERENCES [dbo].[Questionario] ([QuestionarioID]) ON DELETE NO ACTION ON UPDATE NO ACTION;





