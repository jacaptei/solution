using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace JaCaptei.Model {

    public class AppAbout {
        public string name              { get; set; } = Config.settings.apiName;
        public string environment       { get; set; } = Config.settings.environment;
        public float  version           { get; set; } = float.Parse(Config.settings.apiVersion);
        public string release           { get; set; } = Config.settings.release;
    }

    public class AppStatus{
        public string       name        { get; set; } = "SUCCESS";
        public bool         success     { get; set; } = true;
        public int          code        { get; set; } = 200;
        public string       info        { get; set; } = "request processed";
        public bool         hasNotes    { get; set; } = false;
        public int          totalNotes  { get; set; } = 0;
        public List<Note>   notes       { get; set; } = new List<Note>();
        public string       exception   { get; set; } = "";
        public DateTime     date        { get; set; } = DateTime.Now;
    }



    public class Note {

        public int      id          { get; set; } = 0;
        public string   key         { get; set; } = "";
        public string   info        { get; set; } = "";
        public string   complement  { get; set; } = "";

        public Note() {
        }
        public Note(string _key,string _info,string _complement) {
            id = 0;
            key = _key;
            info = _info;
            complement = _complement;
        }
        public Note(string _key,string _info) {
            id = 0;
            key = _key;
            info = _info;
        }
        public Note(int _id,string _key,string _info) {
            id = _id;
            key = _key;
            info = _info;
        }
        public Note(int _id,string _info) {
            id = id;
            key = "";
            info = _info;
        }
        public Note(string _info) {
            key = "";
            id = 0;
            info = _info;
        }

    }
 


    public class AppReturn {

        public AppAbout   about  { get; set; } = new AppAbout();
        public AppStatus  status { get; set; } = new AppStatus();
        public dynamic    result { get; set; } = null;

        public dynamic? ParseResult<T>() {
            return (result is null)? null : System.Text.Json.JsonSerializer.Deserialize<T>(result.ToString());
        }

        public AppReturn() {
            about   = new AppAbout();
            status  = new AppStatus();
            result  = null;
        }

        public AppReturn(dynamic? _data) {
            about   = new AppAbout();
            status  = new AppStatus();
            result  = _data;
        }

        public void Reset() {
            Reset(null);
        }

        public void Reset(Object _data) {
            status.date             =  DateTime.Now;
            status.notes            = new List<Note>();
            status.hasNotes         = false;
            status.success          = true;
            status.code             = (int) HttpStatus.SUCCESS;
            status.name             = HttpStatus.SUCCESS.ToString();
            status.info             = "";
            result                    = _data is null ? new { } : _data;
        }

        public bool HasNotes() {
            return status.hasNotes;
        }


        public void SetAsSuccess(Object _data) {
            status.date             = DateTime.Now;
            status.success          = true;
            status.code             = (int) HttpStatus.SUCCESS;
            status.name             = HttpStatus.SUCCESS.ToString();
            result                  = _data is null ? new { } : _data;
        }

        public void SetAsSuccess() {
            SetAsSuccess(null);
        }


        public void SetAsServerException(string msg, Exception? e) {
            status.success = false;
            result = new { };
            status.code = (int) HttpStatus.SERVER_EXCEPTION;
            status.name = HttpStatus.SERVER_EXCEPTION.ToString();
            status.info = "request failed";
            status.exception = (e is null ? "" : e.ToString());
            AddNote(msg);
        }
        public void SetAsServerException(Exception? e) {
            SetAsServerException("", null);
        }


        public void SetException(string message) {
            AddException(message);
        }
        public void SetAsException(string message) {
            AddException(message);
        }
        public void SetAsException(Exception? e) {
            SetAsBadRequest();
            status.exception    = e is null ? "" : e.ToString();
        }
        public void SetAsException(string message, Exception? e) {
            SetAsBadRequest();
            AddException(message);
            status.exception    = e is null ? "" : e.ToString();
        }
        public void SetAsException() {
            status.success      = false;
            status.code         = (int) HttpStatus.BAD_REQUEST;
            status.name         = HttpStatus.BAD_REQUEST.ToString();
            status.info         = "Não foi possível atender a solicitação.";
            result              = null;
        }

        public void SetAsBadRequest(string note="") {
                status.success      = false;
                status.code         = (int) HttpStatus.BAD_REQUEST;
                status.name         = HttpStatus.BAD_REQUEST.ToString();
                status.info         = "Não foi possível atender a solicitação.";
                result              = null;
                AddNote(  (String.IsNullOrWhiteSpace(note)? "Não foi possível atender a solicitação." : note)  );
        }


        public void SetAsUnauthorized(string note="") {
                status.success      = false;
                status.code         = (int) HttpStatus.UNAUTHORIZED;
                status.name         = HttpStatus.UNAUTHORIZED.ToString();
                status.info         = "Requer autenticação.";
                result                = new Object();
                AddNote(  (String.IsNullOrWhiteSpace(note)?  "Necessário efetuar login para acessar esta área ou este recurso." : note)  );
        }

        public void SetAsForbidden(string note="") {
                status.success      = false;
                status.code         = (int) HttpStatus.FORBIDDEN;
                status.name         = HttpStatus.FORBIDDEN.ToString();
                status.info         = "Acesso não permitido.";
                result                = new Object();
                AddNote(  (String.IsNullOrWhiteSpace(note)? "Necessário permissão para acessar esta área ou este recurso." : note)  );
        }

        public void SetAsNotFound(string note="") {
                status.success      = false;
                status.code         = (int) HttpStatus.NOT_FOUND;
                status.name         = HttpStatus.NOT_FOUND.ToString();
                status.info         = "Nada encontrado.";
                result                = new Object();
                AddNote(  (String.IsNullOrWhiteSpace(note)? "O recurso solicitado não foi encontrado (não cadastrado ou excluído)." : note)  );
        }

        public void SetAsNotAcceptable(string note="") {
                status.success      = false;
                status.code         = (int) HttpStatus.NOT_ACCEPTABLE;
                status.name         = HttpStatus.NOT_ACCEPTABLE.ToString();
                status.info         = "Dados pendentes ou incorretos.";
                result                = new Object();
                AddNote(  (String.IsNullOrWhiteSpace(note)? "Verifique se os dados necessários foram enviados e da forma correta." : note)  );
        }


        public void SetAsGone(string note="") {
                status.success      = false;
                status.code         = (int) HttpStatus.INACTIVATED;
                status.name         = HttpStatus.INACTIVATED.ToString();
                status.info         = "Recurso encontrado porém inativado.";
                AddNote(  (String.IsNullOrWhiteSpace(note)? "O recurso solicitado existe mas não pôde ser obtido pois se encontra inativado." : note)  );
        }

        public void SetAsUnprocessable(string note="") {
                status.success      = false;
                status.code         = (int) HttpStatus.UNPROCESSABLE_ENTITY;
                status.name         = HttpStatus.UNPROCESSABLE_ENTITY.ToString();
                status.info         = "Entidade não pôde ser processada.";
                AddNote(  (String.IsNullOrWhiteSpace(note)? "Ação abortada por pendências ou conflito com algum outro recurso / entidade já existente." : note)  );
        }


        public void AddValidationNote(Note note) {
            status.info = "Pendências informadas nas notas.";
            SetAsException();
            AddNote(note);
        }
        public void AddValidationNote(string key,string _info) {
            status.info = "Pendências informadas nas notas.";
            SetAsException();
            AddNote(key,_info);
        }
        public void AddValidationNote(string _info) {
            status.info = "Pendências informadas nas notas.";
            SetAsException();
            AddNote(status.totalNotes+1,_info);
        }
        public void AddValidationNote(string _info,Exception? e) {
            status.info = "Pendências informadas nas notas.";
            SetAsException(e);
            AddNote(status.totalNotes+1,_info);
        }

        public void AddNote(Note nt) {
            status.hasNotes = true;
            status.totalNotes++;
            nt.id = status.totalNotes;
            status.notes.Add(nt);
        }
        public void AddNote(int index,string info) {
            status.hasNotes = true;
            status.totalNotes++;
            status.notes.Add(new Note(index,info));
        }
        public void AddNote(string key,string info) {
            status.hasNotes = true;
            status.totalNotes++;
            status.notes.Add(new Note(status.totalNotes,key,info));
        }
        public void AddNote(string info) {
            status.hasNotes = true;
            status.totalNotes++;
            status.notes.Add(new Note(status.totalNotes,info));
        }

        public void Notify(string key, string info) {
            AddNote(key,info);
        }
        public void Notify(string info) {
            AddNote(info);
        }


        public void Exception(string key, string _info) {
            AddValidationNote(key,_info);
        }
        public void Exception(string _info) {
            AddValidationNote(_info);
        }
        
        public void AddException(string key, string info) {
            AddValidationNote(key,info);
        }
        public void AddException(string info) {
            AddValidationNote(info);
        }




    }






}
