namespace JaCaptei.Model{


    public class Localidade{

        public Estado           estado          { get; set; } = new Estado ();
        public Cidade           cidade          { get; set; } = new Cidade ();
        public Bairro           bairro          { get; set; } = new Bairro ();

        public List<Estado>     estados         { get; set; } = new List<Estado>();
        public List<Cidade>     cidades         { get; set; } = new List<Cidade>();
        public List<Bairro>     bairros         { get; set; } = new List<Bairro>();

    }

    public class Estado() {
        public int      id          { get; set; }
        public string   uf          { get; set; } = "";
        public string   nome        { get; set; } = "";
        public string   nomeNorm    { get; set; } = "";
        public string   label       { get; set; } = "";
        public int      ibge        { get; set; }
    }


    public class Cidade() {
        public int      id          { get; set; }
        public int      idEstado    { get; set; }
        public string   nome        { get; set; } = "";
        public string   nomeNorm    { get; set; } = "";
        public string   label       { get; set; } = "";
        public string   uf          { get; set; } = "";
        public string   cep         { get; set; } = "";
        public string   ibge        { get; set; } = "";
        public float    area        { get; set; } = 0.0f;
        public int      idMunicipio { get; set; }
    }


    public class Bairro() {
        public int      id          { get; set; }
        public int      idCidade    { get; set; }
        public string   nome        { get; set; } = "";
        public string   nomeNorm    { get; set; } = "";
        public string   label       { get; set; } = "";
    }



}



