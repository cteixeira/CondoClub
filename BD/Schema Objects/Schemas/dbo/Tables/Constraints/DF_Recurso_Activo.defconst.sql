ALTER TABLE [dbo].[Recurso]
    ADD CONSTRAINT [DF_Recurso_Activo] DEFAULT ((1)) FOR [Activo];

