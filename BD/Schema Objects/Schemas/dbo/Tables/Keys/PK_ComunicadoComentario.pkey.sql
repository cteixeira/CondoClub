﻿ALTER TABLE [dbo].[ComunicadoComentario]
    ADD CONSTRAINT [PK_ComunicadoComentario] PRIMARY KEY CLUSTERED ([ComunicadoComentarioID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
