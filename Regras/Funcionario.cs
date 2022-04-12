using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras
{
    public class Funcionario 
    {            

        private _Base<BD.Funcionario> _base = new _Base<BD.Funcionario>();


        public IEnumerable<BD.Funcionario> Lista(string nome, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Funcionario.Where(f => f.CondominioID == utilizador.CondominioID.Value &&
                    (nome == null || f.Nome.Equals(nome))).OrderBy(o => o.Nome).ToList();
            }
        }


        public BD.Funcionario Abrir(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Funcionario obj = ctx.Funcionario.FirstOrDefault(f => f.FuncionarioID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public void Inserir(BD.Funcionario obj, UtilizadorAutenticado utilizador)
        {
            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (Lista(obj.Nome, utilizador).Count() > 0)
                throw new Exceptions.NomeRepetido();

            obj.CondominioID = utilizador.CondominioID.Value;

            using (BD.Context ctx = new BD.Context())
            {
                _base.Inserir(obj, ctx);

                if (obj.FotoID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }
            Notificacao.Processa(obj.FuncionarioID, Notificacao.Evento.NovoFuncionario, utilizador);
        }


        public void Actualizar(BD.Funcionario obj, UtilizadorAutenticado utilizador)
        {
            obj.CondominioID = utilizador.CondominioID.Value;

            Regras.BD.Funcionario original = _base.Abrir(obj.FuncionarioID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            IEnumerable<BD.Funcionario> lista = Lista(obj.Nome, utilizador);
            if (lista.Count() > 0 && lista.Where(x => x.FuncionarioID == obj.FuncionarioID).Count() == 0)
                throw new Exceptions.NomeRepetido();

            ApagaFotoAnterior(obj);

            using (BD.Context ctx = new BD.Context())
            {
                _base.Actualizar(obj, ctx);

                if (obj.FotoID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }
        }


        public void Apagar(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                Regras.BD.Funcionario obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                if (obj.FotoID.HasValue)
                    new Regras.Ficheiro().Apagar(obj.FotoID.Value, ctx);

                _base.Apagar(obj.FuncionarioID, ctx);
                ctx.SaveChanges();
            }
        }


        #region Permissoes

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador)
        {
            if (utilizador.CondominioID.HasValue)
            {
                if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                    utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                    utilizador.Perfil == Regras.Enum.Perfil.Síndico)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
                }

                if (utilizador.Perfil == Regras.Enum.Perfil.Morador ||
                    utilizador.Perfil == Regras.Enum.Perfil.Portaria)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                }
            }

            return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Funcionario obj)
        {
            if (utilizador.CondominioID.HasValue && utilizador.CondominioID.Value == obj.CondominioID)
            {
                if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                    utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                    utilizador.Perfil == Regras.Enum.Perfil.Síndico)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
                }
                if (utilizador.Perfil == Regras.Enum.Perfil.Morador ||
                    utilizador.Perfil == Regras.Enum.Perfil.Portaria)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                }
            }

            return new List<Enum.Permissao>();
        }

        #endregion


        #region Métodos auxiliares

        private bool ObjectoValido(BD.Funcionario obj)
        {
            return !string.IsNullOrEmpty(obj.Nome) && !string.IsNullOrEmpty(obj.Identificacao) &&
                !string.IsNullOrEmpty(obj.Funcao) && !string.IsNullOrEmpty(obj.Horario) &&
                !string.IsNullOrEmpty(obj.Telefone);
        }


        private void ApagaFotoAnterior(BD.Funcionario obj)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Funcionario previous = _base.Abrir(obj.FuncionarioID, ctx);

                if (previous.FotoID.HasValue && previous.FotoID.Value != obj.FotoID.Value) {
                    new Regras.Ficheiro().Apagar(previous.FotoID.Value, ctx);
                }

                ctx.SaveChanges();
            }
        }

        #endregion
    }
}
