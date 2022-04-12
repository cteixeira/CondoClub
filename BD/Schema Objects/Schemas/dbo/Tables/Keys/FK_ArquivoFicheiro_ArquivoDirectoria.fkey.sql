ALTER TABLE [dbo].[ArquivoFicheiro]
    ADD CONSTRAINT [FK_ArquivoFicheiro_ArquivoDirectoria] FOREIGN KEY ([ArquivoDirectoriaID]) REFERENCES [dbo].[ArquivoDirectoria] ([ArquivoDirectoriaID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

