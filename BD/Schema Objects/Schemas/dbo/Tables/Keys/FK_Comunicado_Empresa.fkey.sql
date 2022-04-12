ALTER TABLE [dbo].[Comunicado]
    ADD CONSTRAINT [FK_Comunicado_Empresa] FOREIGN KEY ([EmpresaID]) REFERENCES [dbo].[Empresa] ([EmpresaID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

