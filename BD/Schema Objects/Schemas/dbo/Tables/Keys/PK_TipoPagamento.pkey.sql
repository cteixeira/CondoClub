﻿ALTER TABLE [dbo].[FormaPagamento]
    ADD CONSTRAINT [PK_TipoPagamento] PRIMARY KEY CLUSTERED ([FormaPagamentoID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

