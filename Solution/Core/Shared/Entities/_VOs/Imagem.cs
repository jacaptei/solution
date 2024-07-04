using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {


    public class Imagem {

            public int          id                  {get;set;} = 0;
            public int          index               {get;set;} = 0;
            public int          ordem               {get;set;} = 0;
            public string       nome                {get;set;}="";
                                                    
            public string       cod                 {get;set;}="";
            public string       base64              {get;set;}="";
            public string       url                 {get;set;}="";
            public byte[]       blob                {get;set;}
            
            public string       path                {get;set;}="";
            public string       pathThumb           {get;set;}="";

            public bool        cover                {get;set;}
            public bool        thumb                {get;set;}
            
            public DateTime     data                {get;set;}  = Utils.Date.GetLocalDateTime();

    }



}


