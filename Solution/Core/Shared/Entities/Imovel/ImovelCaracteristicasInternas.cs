using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {
    

    public class ImovelCaracteristicasInternas {

            public int      id                  {get;set;}
            public int      idImovel            {get;set;}

            public short    totalBanheiros      {get;set;}
            public short    totalQuartos        {get;set;}
            public short    totalSalas          {get;set;}
            public short    totalSuites         {get;set;}
            public short    totalVarandas       {get;set;}

            public bool     aguaIndividual      {get;set;}
            public bool     aquecedorGas        {get;set;}
            public bool     aquecedorEletrico   {get;set;}
            public bool     aquecedorSolar      {get;set;}
            public bool     arCondicionado      {get;set;}
            public bool     areaServico         {get;set;}
            public bool     areaPrivativa       {get;set;}
            public bool     armarioBanheiro     {get;set;}
            public bool     armarioCozinha      {get;set;}
            public bool     armarioQuarto       {get;set;}
            public bool     banheiro            {get;set;}
            public bool     boxDespejo          {get;set;}
            public bool     dce                 {get;set;}
            public bool     despensa            {get;set;}
            public bool     closet              {get;set;}
            public bool     churrasqueira       {get;set;}
            public bool     escritorio          {get;set;}
            public bool     gasCanalizado       {get;set;}
            public bool     lavabo              {get;set;}
            public bool     mobilidado          {get;set;}
            public bool     rouparia            {get;set;}
            public bool     quarto              {get;set;}
            public bool     sala                {get;set;}
            public bool     solManha            {get;set;}
            public bool     suite               {get;set;}
            public bool     varanda             {get;set;}
            public bool     varandaGourmet      {get;set;}
            public bool     vistaMar            {get;set;}

    }



}
