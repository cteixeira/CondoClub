﻿ALTER TABLE [dbo].[Localizacao]
    ADD CONSTRAINT [FK_Localizacao_Pais] FOREIGN KEY ([PaisID]) REFERENCES [dbo].[Pais] ([PaisID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

