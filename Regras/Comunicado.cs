using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CondoClub.Regras
{
    public class Comunicado {


        public static IEnumerable<BD.Comunicado> Lista(UtilizadorAutenticado utilizador, int skip, int take) {

            using (BD.Context ctx = new BD.Context()) {

                //perfil condoclub vê todos os comunicados
                //perfil empresa vê comunicados de todos os seus condominios e condoclub
                //restantes perfis apenas vêem do seu condominio, condoclub e empresa que gere o seu condominio (caso exista)

                if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                    return ctx.Comunicado
                        .Include("Utilizador")
                        .Include("ComunicadoComentario")
                        .Include("ComunicadoComentario.Utilizador")
                        .Include("ComunicadoComentario.Utilizador.Condominio")
                        .Include("ComunicadoComentario.Utilizador.Empresa")
                        .Include("Condominio")
                        .Include("Empresa")
                        .OrderByDescending(o => o.ComunicadoID)
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                }
                else if (utilizador.Perfil == Enum.Perfil.Empresa && utilizador.EmpresaID != null) {
                    return ctx.Comunicado
                        .Include("Utilizador")
                        .Include("ComunicadoComentario")
                        .Include("ComunicadoComentario.Utilizador")
                        .Include("ComunicadoComentario.Utilizador.Condominio")
                        .Include("ComunicadoComentario.Utilizador.Empresa")
                        .Include("Condominio")
                        .Include("Empresa")
                        .Where(o =>
                                (o.EmpresaID == null && o.CondominioID == null) ||
                                (o.EmpresaID == utilizador.EmpresaID) ||
                                (o.Condominio.EmpresaID == utilizador.EmpresaID))
                        .OrderByDescending(o => o.ComunicadoID)
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                }
                else if ((
                    utilizador.Perfil == Enum.Perfil.Síndico ||
                    utilizador.Perfil == Enum.Perfil.Morador ||
                    utilizador.Perfil == Enum.Perfil.Portaria ||
                    utilizador.Perfil == Enum.Perfil.Consulta) && 
                    utilizador.CondominioID != null){

                    long? empresaGereCondominioID = ctx.Condominio.FirstOrDefault(o => o.CondominioID == utilizador.CondominioID).EmpresaID;

                    return ctx.Comunicado
                        .Include("Utilizador")
                        .Include("ComunicadoComentario")
                        .Include("ComunicadoComentario.Utilizador")
                        .Include("ComunicadoComentario.Utilizador.Condominio")
                        .Include("ComunicadoComentario.Utilizador.Empresa")
                        .Include("Condominio")
                        .Include("Empresa")
                        .Where(o =>
                                (o.EmpresaID == null && o.CondominioID == null) ||
                                (o.CondominioID == utilizador.CondominioID) ||
                                (empresaGereCondominioID != null && o.EmpresaID == empresaGereCondominioID))
                        .OrderByDescending(o => o.ComunicadoID)
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                }
                else
                    throw new Exceptions.SemPermissao();
            }
        }


        public BD.Comunicado Abrir(long id, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {
                BD.Comunicado obj = ctx.Comunicado
                                        .Include("Utilizador")
                                        .Include("ComunicadoComentario")
                                        .Include("ComunicadoComentario.Utilizador")
                                        .Include("ComunicadoComentario.Utilizador.Condominio")
                                        .Include("ComunicadoComentario.Utilizador.Empresa")
                                        .Include("Condominio")
                                        .Include("Empresa")
                                        .FirstOrDefault(o => o.ComunicadoID == id);

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public long Inserir(BD.Comunicado obj, UtilizadorAutenticado utilizador) {

            if (obj.RemetenteID != utilizador.ID || obj.DataHora == null || obj.Texto == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                
                if (obj.ImagemID.HasValue) 
                    new Ficheiro().Abrir(obj.ImagemID.Value, ctx).Temporario = false;

                ctx.Comunicado.AddObject(obj);
                ctx.SaveChanges();
                Notificacao.Processa(obj.ComunicadoID, Notificacao.Evento.NovoComunicado, utilizador);
                return obj.ComunicadoID;
            }
        }


        public long InserirComentario(BD.ComunicadoComentario obj, UtilizadorAutenticado utilizador) {

            if (obj.ComunicadoID < 1 || obj.ComentadorID != utilizador.ID || 
                obj.DataHora == null || (String.IsNullOrEmpty(obj.Comentario) && obj.Gosto == null))
                throw new Exceptions.DadosIncorrectos();

            using (BD.Context ctx = new BD.Context()) {

                Regras.BD.Comunicado original = ctx.Comunicado.FirstOrDefault(o => o.ComunicadoID == obj.ComunicadoID);

                if (original == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                    throw new Exceptions.SemPermissao();

                ctx.ComunicadoComentario.AddObject(obj);
                ctx.SaveChanges();
                return obj.ComunicadoComentarioID;
            }
        }

        
        public void Apagar(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                
                BD.Comunicado obj = ctx.Comunicado.FirstOrDefault(i => i.ComunicadoID == id);
                
                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                //apagar ficheiro se existir
                if (obj.ImagemID != null) 
                    new Ficheiro().Apagar(obj.ImagemID.Value, ctx);

                ctx.Comunicado.DeleteObject(obj);
                ctx.SaveChanges();
            }
        }


        public void ApagarComentario(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                
                BD.ComunicadoComentario obj = ctx.ComunicadoComentario
                                                .Include("Comunicado")
                                                .FirstOrDefault(o => o.ComunicadoComentarioID == id);
                
                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!PermissoesComentario(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                ctx.ComunicadoComentario.DeleteObject(obj);
                ctx.SaveChanges();
            }
        }


        # region Permissões


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador) {

            if (utilizador.Perfil == Enum.Perfil.CondoClub ||
                utilizador.Perfil == Enum.Perfil.Empresa ||
                utilizador.Perfil == Enum.Perfil.Síndico ||
                utilizador.Perfil == Enum.Perfil.Morador ||
                utilizador.Perfil == Enum.Perfil.Portaria) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }
            else if (
                utilizador.Perfil == Enum.Perfil.Consulta) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }
            else
                return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Comunicado obj) {

            if (utilizador.Perfil == Enum.Perfil.CondoClub ||
                (utilizador.Impersonating && utilizador.PerfilOriginal == Enum.Perfil.CondoClub) ||
                (obj.RemetenteID == utilizador.ID)) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }
            else if (
                utilizador.Perfil == Enum.Perfil.Empresa ||
                utilizador.Perfil == Enum.Perfil.Síndico || 
                utilizador.Perfil == Enum.Perfil.Morador ||
                utilizador.Perfil == Enum.Perfil.Portaria ||
                utilizador.Perfil == Enum.Perfil.Consulta) {

                //Comunicado inserido pelo CondoClub
                if (obj.EmpresaID == null && obj.CondominioID == null) {
                    if (utilizador.Perfil == Enum.Perfil.Consulta)
                        return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                    else
                        return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
                }

                //Comunicado inserido pela empresa
                if (obj.EmpresaID != null) {
                    
                    //Utilizador faz parte da empresa
                    if (obj.EmpresaID == utilizador.EmpresaID)
                        return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };

                    //Utilizadores que fazem parte de condominios geridos pela empresa
                    if (utilizador.CondominioID != null) {
                        using (BD.Context ctx = new BD.Context()) {

                            long? empresaGereCondominioID = ctx.Condominio
                                .FirstOrDefault(o => o.CondominioID == utilizador.CondominioID).EmpresaID;

                            if (obj.EmpresaID == empresaGereCondominioID) {
                                if (utilizador.Perfil == Enum.Perfil.Consulta)
                                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                                else
                                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
                            }
                        }
                    }
                }

                //Comunicado inserido pelo condomínio
                if (obj.CondominioID != null) {
                    
                    //Utilizador faz parte do condominio
                    if (obj.CondominioID == utilizador.CondominioID) {
                        if (utilizador.Perfil == Enum.Perfil.Consulta)
                            return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                        else
                            return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
                    }
                        
                    //Condominio é gerido pela empresa do utilizador
                    if (utilizador.EmpresaID != null) {
                        using (BD.Context ctx = new BD.Context()) {
                            
                            long? empresaGereCondominioID = ctx.Condominio
                                .FirstOrDefault(o => o.CondominioID == obj.CondominioID).EmpresaID;

                            if (utilizador.EmpresaID == empresaGereCondominioID)
                                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
                        }
                    }
                }
            }
                
            return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> PermissoesComentario(UtilizadorAutenticado utilizador, BD.ComunicadoComentario obj) {

            if (utilizador.Perfil == Enum.Perfil.CondoClub ||
                (utilizador.Impersonating && utilizador.PerfilOriginal == Enum.Perfil.CondoClub)) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }
            else if (
                utilizador.Perfil == Enum.Perfil.Empresa ||
                utilizador.Perfil == Enum.Perfil.Síndico ||
                utilizador.Perfil == Enum.Perfil.Morador ||
                utilizador.Perfil == Enum.Perfil.Portaria) {

                if (obj.ComentadorID == utilizador.ID || obj.Comunicado.RemetenteID == utilizador.ID)
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
                else
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar};
            }
            else if (
                utilizador.Perfil == Enum.Perfil.Consulta) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }
            else
                return new List<Enum.Permissao>();
        }


        # endregion
    }

}