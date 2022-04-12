ALTER TABLE [dbo].[ComunicadoComentario]
    ADD CONSTRAINT [FK_ComunicadoComentario_Utilizador] FOREIGN KEY ([ComentadorID]) REFERENCES [dbo].[Utilizador] ([UtilizadorID]) ON DELETE NO ACTION ON UPDATE NO ACTION;



