using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CondoClub.Web.Models
{
    
    public class ConteudoDirectoria
    {
        public long? DirectoriaID { get; set; }
        
        public IList<ArquivoDirectoria> Directorias { get; set; }

        public IList<ArquivoFicheiro> Ficheiros { get; set; }
    }


    public class ArquivoDirectoria
    {
        public ArquivoDirectoria() 
        {
            Permissoes = Regras.ArquivoDirectoria.Permissoes(ControladorSite.Utilizador);
        }

        public ArquivoDirectoria(Regras.BD.ArquivoDirectoria obj)
        {
            ID = obj.ArquivoDirectoriaID;
            CondominioID = obj.CondominioID;
            ArquivoDirectoriaPaiID = obj.ArquivoDirectoriaPaiID;
            Nome = obj.Nome;
            DataHora = obj.DataHora;
            UtilizadorID = obj.UtilizadorID;
            UtilizadorNome = new Regras.Utilizador().Abrir(obj.UtilizadorID).Nome;
            Permissoes = Regras.ArquivoDirectoria.Permissoes(ControladorSite.Utilizador, obj);
        }


        public long? ID { get; set; }

        public long? CondominioID { get; set; }

        public long? ArquivoDirectoriaPaiID { get; set; }

        public string Nome { get; set; }

        public DateTime? DataHora { get; set; }

        public long? UtilizadorID { get; set; }

        public string UtilizadorNome { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public string Link { get; set; }


        public Regras.BD.ArquivoDirectoria ToBDModel()
        {
            Regras.BD.ArquivoDirectoria obj = new Regras.BD.ArquivoDirectoria();
            obj.ArquivoDirectoriaID = (ID != null ? Convert.ToInt64(ID) : obj.ArquivoDirectoriaID);
            obj.CondominioID = (CondominioID != null ? CondominioID.Value : 0);
            obj.ArquivoDirectoriaPaiID = ArquivoDirectoriaPaiID;
            obj.Nome = Nome;
            obj.DataHora = (DataHora != null ? DataHora.Value : DateTime.Now);
            obj.UtilizadorID = (UtilizadorID != null ? UtilizadorID.Value : 0); 
            return obj;

        }
    }


    public class ArquivoFicheiro
    {
        public ArquivoFicheiro() 
        {
            Permissoes = Regras.ArquivoFicheiro.Permissoes(ControladorSite.Utilizador);
        }

        public ArquivoFicheiro(Regras.BD.ArquivoFicheiro obj)
        {
            ID = obj.ArquivoFicheiroID;
            CondominioID = obj.CondominioID;
            ArquivoDirectoriaID = obj.ArquivoDirectoriaID;
            FicheiroID = obj.FicheiroID;
            Nome = obj.Ficheiro.Nome;
            Extensao = obj.Ficheiro.Extensao;
            Comentario = obj.Comentario;
            DataHora = obj.DataHora;
            UtilizadorID = obj.UtilizadorID;
            UtilizadorNome = new Regras.Utilizador().Abrir(obj.UtilizadorID).Nome;
            Tamanho = obj.Ficheiro.Tamanho.HasValue ? Util.ConvertToKB(obj.Ficheiro.Tamanho.Value) : (int?)null;
            CSSClassIcon = Util.GetCssClassFileTypeIcon(obj.Ficheiro.Extensao.ToLower());
            Permissoes = Regras.ArquivoFicheiro.Permissoes(ControladorSite.Utilizador, obj);
        }


        public long? ID { get; set; }

        public long? CondominioID { get; set; }

        public long? ArquivoDirectoriaID { get; set; }

        public long? FicheiroID { get; set; }

        public string Nome { get; set; }

        public string Extensao { get; set; }

        public string Comentario { get; set; }

        public DateTime? DataHora { get; set; }

        public long? UtilizadorID { get; set; }

        public string UtilizadorNome { get; set; }

        public int? Tamanho { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public string Link { get; set; }

        public string CSSClassIcon { get; set; }

        public Regras.BD.ArquivoFicheiro ToBDModel()
        {
            Regras.BD.ArquivoFicheiro obj = new Regras.BD.ArquivoFicheiro();
            obj.ArquivoFicheiroID = (ID != null ? Convert.ToInt64(ID) : obj.ArquivoFicheiroID);
            obj.CondominioID = (CondominioID != null ? CondominioID.Value : 0);
            obj.ArquivoDirectoriaID = ArquivoDirectoriaID;
            obj.FicheiroID = (FicheiroID != null ? FicheiroID.Value : 0);
            obj.Comentario = Comentario;
            obj.DataHora = (DataHora != null ? DataHora.Value : DateTime.Now);
            obj.UtilizadorID = (UtilizadorID != null ? UtilizadorID.Value : 0);
            return obj;
        }
    }


    public class DirectoriaLink
    {
        public long ID { get; set; }

        public string Nome { get; set; }

        public string URL { get; set; }

        public DirectoriaLink(Regras.BD.ArquivoDirectoria obj, string nome, string url)
        {
            ID = obj.ArquivoDirectoriaID;

            if (!string.IsNullOrEmpty(nome))
                Nome = nome;
            else
                Nome = obj.Nome;
            
            URL = url;
        }
    }
}