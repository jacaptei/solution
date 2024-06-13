var submenus = [
    {
        index:0,
        route:'home',
        links:[
            { subviewID:"view1", label:'Fontes'               , actionLog:{ agent:"USER", idAgent:0, area:"SITE", action:"ENTER", keypath:"1-1-0-0-0", page:"home", path:"Home / Fontes"         }},
            { subviewID:"view2", label:'Especialistas'        , actionLog:{ agent:"USER", idAgent:0, area:"SITE", action:"ENTER", keypath:"1-2-0-0-0", page:"home", path:"Home / Especialistas"  }},
            { subviewID:"view3", label:'Financiamento'        , actionLog:{ agent:"USER", idAgent:0, area:"SITE", action:"ENTER", keypath:"1-3-0-0-0", page:"home", path:"Home / Financiamento"  }}
        ]                                                                                                             
    },                                                                                                                
    {                                                                                                                 
        index:1,                                                                                                     
        route:'cases',                                                                                               
         links:[                                                                                                     
            { subviewID:"view1", label:'Intro'                , actionLog:{ agent:"USER", idAgent:0, area:"SITE", action:"ENTER", keypath:"2-1-0-0-0", page:"cases", path:"Estudos de caso / Intro"               }},
            { subviewID:"view2", label:'Estudos de caso'      , actionLog:{ agent:"USER", idAgent:0, area:"SITE", action:"ENTER", keypath:"2-2-0-0-0", page:"cases", path:"Estudos de caso / Estudos de caso"     }},
            { subviewID:"view3", label:'Sobre esta atividade' , actionLog:{ agent:"USER", idAgent:0, area:"SITE", action:"ENTER", keypath:"2-3-0-0-0", page:"cases", path:"Estudos de caso / Sobre esta atividade"}}
        ]
    },
    {
        index:2,
        route:'experts',
         links:[
            /*{ subviewID:"view1", label:'ASCO 22 Highlights'     , actionLog:{ agent:"USER", idAgent:0, area:"SITE",action:"ENTER", keypath:"3-1-0-0", page:"experts", path:"Entrevistas com especialistas / ASCO 22 Highlights"        }},*/
            { subviewID:"view1", label:'Destaques da ASCO'        , actionLog:{ agent:"USER", idAgent:0, area:"SITE",action:"ENTER", keypath:"3-1-0-0-0", page:"experts", path:"Entrevistas com especialistas / Destaques da ASCO"         }},
            /*{ subviewID:"view2", label:'Avanços em Imunoterapia'  , actionLog:{ agent:"USER", idAgent:0, area:"SITE",action:"ENTER", keypath:"3-2-0-0-0", page:"experts", path:"Entrevistas com especialistas / Avanços em Imunoterapia"   }}*/
        ]
    },
    {
        index:3,
        route:'elearning',
         links:[
            { subviewID:"view1", label:'eLearning activities'     , actionLog:{ agent:"USER", idAgent:0, area:"SITE",action:"ENTER", keypath:"4-1-0-0-0", page:"elearning", path:"E-Learning / eLearning activities"   }},
            { subviewID:"view2", label:'Accreditation'            , actionLog:{ agent:"USER", idAgent:0, area:"SITE",action:"ENTER", keypath:"4-2-0-0-0", page:"elearning", path:"E-Learning / Accreditation"          }},
            { subviewID:"view3", label:'About these activities'   , actionLog:{ agent:"USER", idAgent:0, area:"SITE",action:"ENTER", keypath:"4-3-0-0-0", page:"elearning", path:"E-Learning / About these activities" }}
        ]
    },
    {
        index:4,
        route:'news',
        links:[]
    },
    {
        index:5,
        route:'password',
        links:[]
    },
    {
        index:6,
        route:'iframe',
        links:[]
    },
    {
        index:7,
        route:'',
        links:[]
    },
    {
        index:8,
        route:'',
        links:[]
    },
    {
        index:9,
        route:'',
        links:[]
    },
    {
        index:10,
        route:'',
        links:[]
    }

];


window.onscroll = function () {
    //console.log($("#submenu").offsetTop + " - " + window.pageYOffset);
    if (window.pageYOffset > 355) {
        //$("#submenu").removeClass("submenu");
        $("#submenu").addClass("submenu-sticky");
    } else {
        $("#submenu").removeClass("submenu-sticky");
        //$("#submenu").addClass("submenu");
    }
};


