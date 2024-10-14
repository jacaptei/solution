namespace JaCaptei.Model{


    public class Busca{

        public dynamic          item            { get; set; }

        public int              page            { get; set; } = 1;
        
        private int             _resultsPerPage;
        public int              resultsPerPage {
                                                get { return _resultsPerPage;                   }
                                                set { _resultsPerPage = (value <=0)? 1 : value; }
        }

        public int              offset          { get => ((page-1) * resultsPerPage); }
        public string           status          { get; set; } = "";
        public int              idStatus        { get; set; }

        public string           name            { get; set; } = "";
        public string           document        { get; set; } = "";
        
        public int[]            mins            { get; set; } = new int[10];
        public int[]            maxs            { get; set; } = new int[10];

        public bool             todos           { get; set; } = false;

        public DateTime[]       datesIn         { get; set;} = new DateTime[10];
        public DateTime[]       datesOut        { get; set;} = new DateTime[10];        

        public DateTime         dateFrom        { get; set;} = Utils.Date.GetUnsetDefaultDateTime();
        public DateTime         dateTo          { get; set;} = Utils.Date.GetLocalDateTime();

        public Int64            total           { get; set; }
        public dynamic          result          { get; set; }
        public List<dynamic>    results         { get; set; } = new List<dynamic>();

        public string           orderBy         { get; set; } = " id DESC ";
        
        public string           filter          { get; set; }


        public Busca() {
            resultsPerPage = 20;
        }


    }


    public class SearchSolicitacoes:Busca {

            public int totalAguardando  { get; set; }
            public int totalVerificando { get; set; }
            public int totalFinalizado  { get; set; }


    }






}



