ALTER TABLE [dbo].[PrecoCondominio]
    ADD CONSTRAINT [FK_PrecoCondominio_Pais] FOREIGN KEY ([PaisID]) REFERENCES [dbo].[Pais] ([PaisID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

