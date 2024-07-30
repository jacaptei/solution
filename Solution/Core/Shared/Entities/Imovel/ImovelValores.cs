using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JaCaptei.Model {
    

    public class ImovelValores{

            public int      id                {get;set;}
            public int      idImovel          {get;set;}

            public bool     sobConsulta       {get;set;}

            public float   venda             {get;set;} = 0;
            public float   vendaAnterior     {get;set;} = 0;
            public float   aluguel           {get;set;} = 0;
            public float   aluguelAnterior   {get;set;} = 0;
            public float   condominio        {get;set;} = 0;
            public float   consulta          {get;set;} = 0;
            public float   iptuAnual         {get;set;} = 0;
            public float   iptuMensal        {get;set;} = 0;
            public float   iptuIndice        {get;set;} = 0;
            public float   comissao          {get;set;} = 0; // %
            public float   rentabilidade     {get;set;} = 0; // %
            public float   maximo            {get;set;} = 0;
            public float   minimo            {get;set;} = 0;

    }



}


