﻿ALTER TABLE [dbo].[ArquivoDirectoria]
    ADD CONSTRAINT [PK_ArquivoDirectoria] PRIMARY KEY CLUSTERED ([ArquivoDirectoriaID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
