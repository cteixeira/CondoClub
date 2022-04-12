using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras
{
    public class Veiculo
    {

        private _Base<BD.Veiculo> _base = new _Base<BD.Veiculo>();

        public IEnumerable<BD.Veiculo> Lista(string matricula, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Veiculo.Where(v => v.CondominioID == utilizador.CondominioID.Value &&
                    (matricula == null || v.Matricula.Equals(matricula))).ToList();
            }
        }


        public BD.Veiculo Abrir(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Veiculo obj = ctx.Veiculo.FirstOrDefault(v => v.VeiculoID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public void Inserir(BD.Veiculo obj, UtilizadorAutenticado utilizador)
        {
            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (Lista(obj.Matricula, utilizador).Count() > 0)
                throw new Exceptions.MatriculaRepetida();

            obj.CondominioID = utilizador.CondominioID.Value;

            using (BD.Context ctx = new BD.Context())
            {
                _base.Inserir(obj, ctx);

                if (obj.FotoID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }
            Notificacao.Processa(obj.VeiculoID, Notificacao.Evento.NovoVeiculo, utilizador);
        }


        public void Actualizar(BD.Veiculo obj, UtilizadorAutenticado utilizador)
        {
            obj.CondominioID = utilizador.CondominioID.Value;

            Regras.BD.Veiculo original = _base.Abrir(obj.VeiculoID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            

            IEnumerable<BD.Veiculo> lista = Lista(obj.Matricula, utilizador);
            if (lista.Count() > 0 && lista.Where(x => x.VeiculoID == obj.VeiculoID).Count() == 0)
                throw new Exceptions.MatriculaRepetida();

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
                Regras.BD.Veiculo obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                if (obj.FotoID.HasValue)
                    new Regras.Ficheiro().Apagar(obj.FotoID.Value, ctx);
                
                _base.Apagar(obj.VeiculoID, ctx);
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


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Veiculo obj)
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

        private bool ObjectoValido(BD.Veiculo obj)
        {
            return !string.IsNullOrEmpty(obj.Matricula) && !string.IsNullOrEmpty(obj.Marca) && !string.IsNullOrEmpty(obj.Modelo);
        }


        private void ApagaFotoAnterior(BD.Veiculo obj)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Veiculo previous = _base.Abrir(obj.VeiculoID, ctx);

                if (previous.FotoID.HasValue && previous.FotoID.Value != obj.FotoID.Value)
                    new Regras.Ficheiro().Apagar(previous.FotoID.Value, ctx);

                ctx.SaveChanges();
            }
        }

        #endregion
    }
}
