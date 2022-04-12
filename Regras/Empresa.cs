using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CondoClub.Regras.Exceptions;

namespace CondoClub.Regras {
    public enum EmpresaPermissao {
        Visualizar,
        Gravar,
        Apagar,
        EnviarConvites,
        ActualizarPerfil
    }

    public enum EmpresaEstado {
        Activo,
        Inactivo,
        PorValidar
    }

    public class Empresa {

        _Base<BD.Empresa> _base = new _Base<BD.Empresa>();

        public IEnumerable<BD.Empresa> Lista(UtilizadorAutenticado utilizador) {

            if (utilizador.Perfil != Enum.Perfil.CondoClub) {
                throw new SemPermissao();
            }

            return _base.Lista();
        }

        public IEnumerable<BD.Empresa> Pesquisa(long? empresaID, EmpresaEstado? estado, string termo, int skip,
            int take, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(EmpresaPermissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Empresa
                    .Include("Pais")
                    .Include("Utilizador")
                    .Where(e =>
                        (!estado.HasValue ||
                            (estado == EmpresaEstado.Activo && e.Activo) ||
                            (estado == EmpresaEstado.Inactivo && !e.Activo && e.DataActivacao.HasValue) ||
                            (estado == EmpresaEstado.PorValidar && !e.Activo && !e.DataActivacao.HasValue)
                        ) &&
                        (string.IsNullOrEmpty(termo) || e.Nome.Contains(termo) || e.Contribuinte.Contains(termo) ||
                        e.Cidade.Contains(termo) || e.Estado.Contains(termo))
                    )
                    .OrderBy(e => e.Nome)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }
        }

        public BD.Empresa Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.Empresa obj = ctx.Empresa
                    .Include("Pais")
                    .Include("Utilizador")
                    .FirstOrDefault(e => e.EmpresaID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(EmpresaPermissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }

        public string Nome(long id, UtilizadorAutenticado utilizador) {
            if (!utilizador.EmpresaID.HasValue || id != utilizador.EmpresaID.Value)
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Empresa.FirstOrDefault(e => e.EmpresaID == id).Nome;
            }
        }

        public void Inserir(BD.Empresa obj, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(EmpresaPermissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaNomeRepetido(obj.EmpresaID, obj.Nome))
                throw new Exceptions.EmailRepetido();

            using (BD.Context ctx = new BD.Context()) {

                //verificar se tem utilizador associado para poder activar
                if (obj.Activo && (obj.Utilizador == null || obj.Utilizador.Count == 0)) {
                    throw new Exceptions.EmpresaSemUtilizador();
                }

                obj.DataCriacao = DateTime.Now;
                
                _base.Inserir(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }
        }

        public void Registar(BD.Empresa obj) {
            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            using (BD.Context ctx = new BD.Context()) {
                if (obj.EmpresaID == 0) {
                    if (ctx.Empresa.Where(e => e.Nome.Equals(obj.Nome)).ToList().Count > 0)
                        throw new Exceptions.DesignacaoRepetida();

                    obj.DataCriacao = DateTime.Now;

                    _base.Inserir(obj, ctx);
                } else
                    _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }
        }

        public BD.Empresa RegistarRetroceder(long id) {
            Regras.BD.Empresa obj = _base.Abrir(id);

            if (obj == null || obj.Activo)
                throw new Regras.Exceptions.DadosIncorrectos();

            return obj;
        }

        public void Actualizar(BD.Empresa obj, UtilizadorAutenticado utilizador) {

            BD.Empresa original = _base.Abrir(obj.EmpresaID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(EmpresaPermissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaNomeRepetido(obj.EmpresaID, obj.Nome))
                throw new Exceptions.NomeRepetido();

            if (!original.Activo && obj.Activo)
                obj.DataActivacao = DateTime.Now;

            using (BD.Context ctx = new BD.Context()) {

                if (!original.Activo && obj.Activo) {

                    //verificar se tem utilizador associado (e activo) para poder activar
                    if (obj.Activo && !ctx.Utilizador.Any(u => u.PerfilUtilizadorID == (int)Enum.Perfil.Empresa && u.EmpresaID == obj.EmpresaID && u.Activo)) {
                        throw new Exceptions.EmpresaSemUtilizador();
                    }

                    obj.DataActivacao = DateTime.Now;

                }

                //Apagar foto anterior
                if (original.AvatarID.HasValue && original.AvatarID.Value != obj.AvatarID.Value)
                    new Regras.Ficheiro().Apagar(original.AvatarID.Value, ctx);

                //Data de criação não é editavel
                obj.DataCriacao = original.DataCriacao;

                _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();

                if (!original.DataActivacao.HasValue && obj.Activo) {
                    try {
                        Notificacao.Processa(obj.EmpresaID, Notificacao.Evento.NovaEmpresaActiva, utilizador);
                    } catch { throw; }
                }
            }
        }

        public void ActualizarPerfil(BD.Empresa obj, UtilizadorAutenticado utilizador) {
            BD.Empresa original = _base.Abrir(obj.EmpresaID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(EmpresaPermissao.ActualizarPerfil))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaNomeRepetido(obj.EmpresaID, obj.Nome))
                throw new Exceptions.NomeRepetido();

            SubreporCamposPerfilReadonly(obj);

            using (BD.Context ctx = new BD.Context()) {

                //Apagar foto anterior
                if (original.AvatarID.HasValue && original.AvatarID.Value != obj.AvatarID.Value)
                    new Regras.Ficheiro().Apagar(original.AvatarID.Value, ctx);

                //Data de criação não é editavel
                obj.DataCriacao = original.DataCriacao;

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }
        }

        public void Apagar(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                Regras.BD.Empresa obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(EmpresaPermissao.Apagar))
                    throw new Exceptions.SemPermissao();

                if (obj.Condominio.Any() || obj.Comunicado.Any())
                    throw new Exceptions.TemDependencias();

                if (obj.AvatarID.HasValue)
                    new Regras.Ficheiro().Apagar(obj.AvatarID.Value, ctx);

                //Apagar os utilizadores
                foreach (var ut in ctx.Utilizador.Where(ut => ut.EmpresaID.HasValue && ut.EmpresaID.Value == id).ToList()) {
                    foreach (var msgDest in ut.MensagemDestinatario.ToList()) {
                        ctx.DeleteObject(msgDest);
                    }
                    foreach (var msg in ut.Mensagem.ToList()) {
                        ctx.DeleteObject(msg);
                    }
                    ctx.DeleteObject(ut);
                }

                _base.Apagar(obj.EmpresaID, ctx);
                ctx.SaveChanges();
            }
        }

        #region Métodos auxiliares

        private bool ObjectoValido(BD.Empresa obj) {
            return !string.IsNullOrEmpty(obj.Nome) && !string.IsNullOrEmpty(obj.Endereco) &&
                !string.IsNullOrEmpty(obj.Cidade) && !string.IsNullOrEmpty(obj.CodigoPostal) &&
                obj.PaisID != 0;
        }

        private bool VerificaNomeRepetido(long? id, string nome) {
            using (BD.Context ctx = new BD.Context()) {
                return ctx.Empresa.Any(i => (id == null || i.EmpresaID != id) && i.Nome == nome);
            }
        }

        private void SubreporCamposPerfilReadonly(BD.Empresa obj) {
            BD.Empresa objAnterior = _base.Abrir(obj.EmpresaID);
            obj.Activo = objAnterior.Activo;
            obj.DataActivacao = objAnterior.DataActivacao;
            obj.Endereco = objAnterior.Endereco;
            obj.Localidade = objAnterior.Localidade;
            obj.Cidade = objAnterior.Cidade;
            obj.CodigoPostal = objAnterior.CodigoPostal;
            obj.Estado = objAnterior.Estado;
            obj.PaisID = objAnterior.PaisID;
            obj.Latitude = objAnterior.Latitude;
            obj.Longitude = objAnterior.Longitude;
        }

        #endregion

        #region Permissões

        public static List<EmpresaPermissao> Permissoes(UtilizadorAutenticado utilizador) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                return new List<EmpresaPermissao>() { EmpresaPermissao.Visualizar, EmpresaPermissao.Gravar, 
                    EmpresaPermissao.EnviarConvites };
            }

            if (utilizador.Perfil == Enum.Perfil.Empresa) {
                return new List<EmpresaPermissao>() { EmpresaPermissao.Visualizar };
            }

            return new List<EmpresaPermissao>();
        }

        public static List<EmpresaPermissao> Permissoes(UtilizadorAutenticado utilizador, BD.Empresa obj) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                return new List<EmpresaPermissao>() { EmpresaPermissao.Visualizar, EmpresaPermissao.Gravar, EmpresaPermissao.Apagar };
            }

            if (utilizador.Perfil == Enum.Perfil.Empresa && utilizador.EmpresaID.HasValue &&
                utilizador.EmpresaID.Value == obj.EmpresaID) {
                return new List<EmpresaPermissao>() { EmpresaPermissao.Visualizar, EmpresaPermissao.ActualizarPerfil };
            }

            return new List<EmpresaPermissao>();
        }

        #endregion

    }
}
