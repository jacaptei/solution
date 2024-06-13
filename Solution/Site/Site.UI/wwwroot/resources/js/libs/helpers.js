

/* ******************************************* 
                MIX
******************************************* */


//const unbind = function(val){ return JSON.stringify(JSON.parse(val));}
const Unbind = function(val){ return JSON.parse(JSON.stringify(val));}

const IsSet = function(val) {
    return (val && val != null && val !== null && val !== "null" && val != "null" && val != "" && typeof val != undefined && typeof val !== undefined && typeof val != 'undefined' && typeof val != 'UNDEFINED' && typeof val !== 'undefined' && typeof val !== 'UNDEFINED');
}
const is = function(val){
    return this.IsSet(val);
}
const not = function(val){
    return !this.IsSet(val);
}
const IsObject = function(val) {
    return (typeof value === 'object');
} 

const IsJsonObject = function(val) {
    try {
        JSON.stringify(val);
        return true;
    } catch (e) {
        return false;
    }
}

 const IsJsonText = function(str) {
    try {
        JSON.parse(str);
        return true;
    } catch (e) {
        return false;
    }
}






/* ******************************************* 
                CONSOLE
******************************************* */

let defaultFontSize="1.1em";
let defaultFontColor="#58CFFA";

const carray = function(arr){
    var style = "color:#00b3ef;font-size:1em;padding:1px";
    arr.forEach((i)=>console.info("%c "+i, style));
}

const CustomConsoleLog = function(type,val1,val2,size,color,bgcolor){
    //var style = "color:"+color+";background:#282828;font-size:"+size+";margin:20px;margin-bottom:30px;padding:20px;border: 1px solid "+color;
    var style = "color:"+color+";font-size:"+size+";margin:20px;margin-bottom:30px;padding:20px;border: 1px solid "+color;
       val1 = IsJsonObject(val1)? JSON.stringify(val1) : val1;
       val2 = IsJsonObject(val2)? JSON.stringify(val2) : val2;
       if(type != 'OUTPUT'){
           if(IsSet(val1))
               console.info("%c [ "+type+" ] "+val1 +" = "+ val2, style);
            else
               console.info("%c [ "+type+" ] "+val2, style);
       }else{
           if(IsSet(val1))
               console.info("%c"+val1 +" = "+ val2, style);
            else
               console.info("%c"+val2, style);
       }
}






//const CustomConsoleLog = function(type,val1,val2,size,color,bgcolor){
//    //var style = "color:"+color+";background:"+bgcolor;
//    var style = "color:"+color+";font-size:"+size+";margin:20px;margin-bottom:30px;padding:20px;border: 1px solid "+color;
//    //console.log(("%c \n\n"+type), style)
//   if(IsSet(val1))
//       val1 = (IsJsonObject(val1) || IsObject(val1))? JSON.stringify(val1) : val1;
//   if(IsSet(val2)){
//       val2 = (IsJsonObject(val2) || IsObject(val2))? JSON.stringify(val2) : val2;
//       //console.log("%c [ "+type+" ] "+val1 +val2, style);
//       console.info("%c [ "+type+" ] "+val1 +" = "+ val2, style);
//    }else{
//       //console.log("%c [ "+type+" ] "+val1, style);
//       console.info("%c [ "+type+" ] "+val1, style);
//    }
//}

 
const craw = function(val1,val2){ 
    //var style = "color:"+color+";background:"+bgcolor;
    var style = "color:#6AFDFF;background:#282828;font-size:"+defaultFontSize+";margin-top:5px;margin-top:30px;padding:5px;border-bottom: 1px solid #6AFDFF";
    console.info("%c                   RAW                   ", style);
   if(IsSet(val1))
       console.info(val1);
   if(IsSet(val2))
       console.info(val1);
    style = "color:#6AFDFF;background:#282828;font-size:"+defaultFontSize+";margin-top:10px;margin-bottom:30px;padding:5px;border-top: 1px solid #6AFDFF";
    console.info("%c //                                      ", style);
 };
 

const c = function(val){ 
    CustomConsoleLog("OUTPUT",null,val,defaultFontSize,"#00b3ef","#000");
 };
 
const ci = function(val){ 
    CustomConsoleLog("INFO",null,val,defaultFontSize,"#43D8F9","#000");
 };
 
const cl = function(val){ 
    CustomConsoleLog("LOG",null,val,defaultFontSize,"#FFF","#000");
};
 
const cs = function(val){ 
    CustomConsoleLog("SUCCESS",null,val,defaultFontSize,"#55FF7F","#000");
 };

const cw = function(val){ 
    CustomConsoleLog("WARNING",null,val,defaultFontSize,"#FFFF00","#000");
 };

const ce = function(val){ 
    CustomConsoleLog("ERROR",null,val,defaultFontSize,"#FF6AEC","#000");
    craw(val);
 };

const cred = function(val){ 
    CustomConsoleLog("OUTPUT",null,val,defaultFontSize,"#FF6AEC","#000");
    craw(val);
 };

const cgreen = function(val){ 
    CustomConsoleLog("OUTPUT",null,val,defaultFontSize,"#289A48","#000");
    craw(val);
 };


 
const c2 = function(val1,val2){ 
    CustomConsoleLog("OUTPUT",val1,val2,defaultFontSize,"#58CFFA","#000");
 };
 
const ci2 = function(val1,val2){ 
    CustomConsoleLog("INFO",val1,val2,defaultFontSize,"#43D8F9","#000");
 };
 
const cl2 = function(val1,val2){ 
    CustomConsoleLog("LOG",val1,val2,defaultFontSize,"#FFF","#000");
};
 
const cs2 = function(val1,val2){ 
    CustomConsoleLog("SUCCESS",val1,val2,defaultFontSize,"#55FF7F","#000");
 };

const cw2 = function(val1,val2){ 
    CustomConsoleLog("WARNING",val1,val2,defaultFontSize,"#FFFF00","#000");
 };

const ce2 = function(val1,val2){ 
    CustomConsoleLog("ERROR",val1,val2,defaultFontSize,"#FF6AEC","#000");
    craw(val1);
 };





 const cclear=function(){console.clear();}



 /*
 
const c = function(val1,val2){ 
    var msg="";
    if(IsSet(val1))
       msg += IsJsonObject(val1)? JSON.stringify(val1) : val1 ;
    if(IsSet(val2))
       msg += "\u001b[2K\u001b[0E" + IsJsonObject(val2)? JSON.stringify(val2) : val2 ;

       console.info(msg);

 };

 */












