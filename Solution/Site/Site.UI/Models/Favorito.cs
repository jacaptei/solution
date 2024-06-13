namespace JaCaptei.UI.Models {

    public class Favorito {
        public int      id            { get; set; } = 0;
        public string   sessao        { get; set; } = "";
        public int      idUsuario     { get; set; } = 0;
        public int      idImovel      { get; set; } = 0;
        public string   idUsuarioCRM  { get; set; } = "";
        public string   idImovelCRM   { get; set; } = "";
        public string   value         { get; set; } = "";
        public bool     adicionar     { get; set; } = true;
        public DateTime data          { get; set; } = Utils.Date.GetLocalDateTime();
    }

}
