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

            public double   atual             {get;set;} = 0d;
            public double   venda             {get;set;} = 0d;
            public double   vendaAnterior     {get;set;} = 0d;
            public double   aluguel           {get;set;} = 0d;
            public double   aluguelAnterior   {get;set;} = 0d;
            public double   condominio        {get;set;} = 0d;
            public double   consulta          {get;set;} = 0d;
            public double   iptuMensal        {get;set;} = 0d;
            public double   iptuAnual         {get;set;} = 0d;
            public double   comissao         {get;set;} = 0d; // %
            public double   rentabilidade    {get;set;} = 0d; // %
            public double   maximo            {get;set;} = 0d;
            public double   minimo            {get;set;} = 0d;

    }



}


