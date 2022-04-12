ALTER TABLE [dbo].[ComunicadoComentario]
    ADD CONSTRAINT [FK_ComunicadoComentario_Comunicado] FOREIGN KEY ([ComunicadoID]) REFERENCES [dbo].[Comunicado] ([ComunicadoID]) ON DELETE CASCADE ON UPDATE NO ACTION;



