ALTER TABLE [dbo].[PrecoCondominio]
    ADD CONSTRAINT [FK_PrecoCondominio_ExtratoSocial] FOREIGN KEY ([ExtratoSocialID]) REFERENCES [dbo].[ExtratoSocial] ([ExtratoSocialID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

