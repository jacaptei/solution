using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {


    public class ImovelImagem {

            public int          id                  {get;set;}=0;
            public int          idImovel            {get;set;}=0;
            public string       cod                 {get;set;}="";
            public string       nome                {get;set;}="";
            public string       arquivo             {get;set;}="";
            public string       arquivoOriginal     {get;set;}="";
            
            public string       tipo                {get;set;}="jpg";
            public string       contentType         {get;set;}="img/jpeg";
            public short        index               {get;set;}=0;
            public short        ordem               {get;set;}=0;
            public short        width               {get;set;}=0;
            public short        height              {get;set;}=0;
            public int          size                {get;set;}=0;
            public int          version             {get;set;}=0;
            public string       base64              {get;set;}="";

            public bool         principal           {get;set;}=false;
            public string       url                 {get;set;}="";
            public string       urlThumb            {get;set;}="";
            public string       urlSmall            {get;set;}="";
            public string       urlMedium           {get;set;}="";
            public string       urlLarge            {get;set;}="";
            public string       urlFull             {get;set;}="";
            public string       urlFlex             {get;set;}="";
            public string       urlLegado           {get;set;}="";

            public string       vendor              {get;set;}="IMAGESHACK"; 
            public string       server              {get;set;}="";
            public string       bucket              {get;set;}="";

            public string       tag                 {get;set;}="";
            public long         tokenNum            {get;set;}=0;

            public DateTime     data                {get;set;}  = Utils.Date.GetLocalDateTime();

    }



}


