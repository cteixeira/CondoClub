﻿ALTER TABLE [dbo].[Funcionario]
    ADD CONSTRAINT [PK_Funcionario] PRIMARY KEY CLUSTERED ([FuncionarioID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

