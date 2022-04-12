ALTER TABLE [dbo].[ArquivoDirectoria]
    ADD CONSTRAINT [FK_ArquivoDirectoria_ArquivoDirectoria1] FOREIGN KEY ([ArquivoDirectoriaPaiID]) REFERENCES [dbo].[ArquivoDirectoria] ([ArquivoDirectoriaID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

