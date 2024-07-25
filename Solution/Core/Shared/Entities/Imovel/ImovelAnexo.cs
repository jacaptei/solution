using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {


    public class ImovelAnexo {

            public int          id                  {get;set;}=0;
            public int          idImovel            {get;set;}=0;
            public string       nome                {get;set;}="";
            public string       arquivo             {get;set;}="";
            public string       arquivoOriginal     {get;set;}="";
            public string       tipo                {get;set;}="";
            public string       contentType         {get;set;}="";
            public short        index               {get;set;}=0;
            public short        ordem               {get;set;}=0;
            public short        width               {get;set;}=0;
            public short        height              {get;set;}=0;
            public int          size                {get;set;}=0;
            public int          version             {get;set;}=0;
            public string       base64              {get;set;}="";

            public string       tag                 {get;set;}="";
            public long         tokenNum            {get;set;}=0;

            public DateTime     data                {get;set;}  = Utils.Date.GetLocalDateTime();

    }



}


