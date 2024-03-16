using System.Text.Json;

namespace Funzionalità
{
    class Configurazione
    {
        // Campi
        JsonSerializerOptions options;

        // dati database locale
        const string server = "localhost";
        const string database = "airline_manager";
        const string username = "root";
        const string password = "";

        string conString;
        List<string> urlList;
        Dictionary<string, string> path;

//GV----------------------------------------------------------------------------------------------------------------
        const short numeroPostiRicorrenza = 90;
        const short limiteRisultati = 5;
        const short inizioCheckIn = -2;
        const short fineCheckIn = -1;
        const short fineCancellazione = -2;

        // elenco sovrapprezzi
        const int sovrapprezzoNumeroPostiOccupati1 = 10;
        const int sovrapprezzoNumeroPostiOccupati2 = 20;
        const int sovrapprezzoNumeroPostiOccupati3 = 30;
        const int sovrapprezzoBassaStagione = 5;
        const int sovrapprezzoMediaStagione = 20;
        const int sovrapprezzoAltaStagione = 40;

        const string anno = "2024"; 
        DateTime inizioBassaStagione = DateTime.Parse(anno + "-01-01"); // bassa stagione: GEN, FEB, MAR, APR
        DateTime fineBassaStagione = DateTime.Parse(anno + "-04-30");
        DateTime inizioMediaStagione = DateTime.Parse(anno + "-09-01"); // media stagione: SEP, OCT, NOV, DIC
        DateTime fineMediaStagione = DateTime.Parse(anno + "-12-31");
        DateTime inizioAltaStagione = DateTime.Parse(anno + "-05-01"); // alta stagione: MAG, GIU, LUG, AGO
        DateTime fineAltaStagione = DateTime.Parse(anno + "-08-30");

        // elenco URL
//GV----------------------------------------------------------------------------------------------------------------
        const string urlCercaVoliDataIata = "http://localhost:5000/utente/voli/ricerca/";
        const string urlInserisciPrenotazione = "http://localhost:5000/utente/prenotazioni/inserisci/";
        const string urlInserisciPasseggero = "http://localhost:5000/utente/passeggeri/inserisci/";
        const string urlModificaRicorrenzaNumeroPostiOccupati = "http://localhost:5000/utente/ricorrenze/modifica/numeroPostiOccupati/";
        const string urlCheckIn = "http://localhost:5000/utente/checkIn/";
        const string urlCancellaPrenotazione = "http://localhost:5000/utente/prenotazioni/elimina/";
        const string urlVisualizzaCartaImbarco = "http://localhost:5000/utente/carteImbarco/visualizza/";
//RR97--------------------------------------------------------------------------------------------------------------
        const string urlLoginAdmin = "http://localhost:5000/admin/login/";
        const string urlInserisciVolo = "http://localhost:5000/admin/volo/inserisci/";
        const string urlRicercaVolo = "http://localhost:5000/admin/volo/ricerca/";
        const string urlEliminaVolo = "http://localhost:5000/admin/volo/elimina/";
        const string urlInserisciRicorrenza = "http://localhost:5000/admin/ricorrenza/inserisci/";
        const string urlEliminaRicorrenza = "http://localhost:5000/admin/ricorrenza/elimina/";
        const string urlModificaRicorrenza = "http://localhost:5000/admin/ricorrenza/modifica/";
        const string urlModificaBagaglio = "http://localhost:5000/utente/modifica/bagaglio/";
        const string urlRichiestaPosto = "http://localhost:5000/utente/richiesta/posto/";
        const string urlAssegnaPosto = "http://localhost:5000/utente/assegna/posto/";
//------------------------------------------------------------------------------------------

        // Costruttore
        public Configurazione()
        {
            options = new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true};

            this.conString = "server = " + server + ";" + "database = " + database + ";" + "username = " + username + ";" + "password = " + password + ";";
            
            urlList = new List<string>()
            {
//GV----------------------------------------------------------------------------------------------------------------
                urlCercaVoliDataIata,
                urlInserisciPrenotazione,
                urlInserisciPasseggero,
                urlModificaRicorrenzaNumeroPostiOccupati,
                urlCheckIn,
                urlCancellaPrenotazione,
                urlVisualizzaCartaImbarco,
//RR97--------------------------------------------------------------------------------------------------------------
                urlLoginAdmin,
                urlInserisciVolo,
                urlRicercaVolo,
                urlEliminaVolo,
                urlInserisciRicorrenza,
                urlEliminaRicorrenza,
                urlModificaRicorrenza,
                urlModificaBagaglio,
                urlRichiestaPosto,
                urlAssegnaPosto
 //------------------------------------------------------------------------------------------------------------
            };

            // path viene sfruttato in Server.cs -> Main
            path = new Dictionary<string, string>()
            {
 //RR97--------------------------------------------------------------------------------------------------------------
                {"pathLoginAdmin", "/admin/login/"},
                {"pathInserisciVolo", "/admin/volo/inserisci/"},
                {"pathRicercaVolo", "/admin/volo/ricerca/"},
                {"pathEliminaVolo", "/admin/volo/elimina/"},
                {"pathInserisciRicorrenza", "/admin/ricorrenza/inserisci/"},
                {"pathEliminaRicorrenza", "/admin/ricorrenza/elimina/"},
                {"pathModificaRicorrenza", "/admin/ricorrenza/modifica/"},
                {"pathModificaBagaglio", "/utente/modifica/bagaglio/"},
                {"pathRichiestaPosto", "/utente/richiesta/posto/"},
                {"pathAssegnaPosto", "/utente/assegna/posto/"},
 //GV--------------------------------------------------------------------------------------------------------------------------
                {"pathCercaVoliDataIata", "/utente/voli/ricerca/"},
                {"pathInserisciPrenotazione", "/utente/prenotazioni/inserisci/"},
                {"pathInserisciPasseggero", "/utente/passeggeri/inserisci/"},
                {"pathModificaRicorrenzaNumeroPostiOccupati", "/utente/ricorrenze/modifica/numeroPostiOccupati/"},
                {"pathCheckIn", "/utente/checkIn/"},
                {"pathCancellaPrenotazione", "/utente/prenotazioni/elimina/"},
                {"pathVisualizzaCartaImbarco", "/utente/carteImbarco/visualizza/"}
            };
        }


        // Proprietà
        public JsonSerializerOptions Options
        {
            get { return options; }
        }

        public string Constring
        {
            get { return this.conString; }
        }

        public List<string> UrlList
        {
            get { return urlList; }
        }

        public Dictionary<string,string> Path
        {
            get { return path; }
        }

        public short LimiteRisultati
        {
            get { return limiteRisultati; }
        }

        public short NumeroPostiRicorrenza
        {
            get { return numeroPostiRicorrenza; }
        }

        public short InizioCheckIn
        {
            get { return inizioCheckIn; }
        }

        public short FineCheckIn
        {
            get { return fineCheckIn; }
        }

        public short FineCancellazione
        {
            get { return fineCancellazione; }
        }

        public int SovrapprezzoNumeroPostiOccupati1
        {
            get { return sovrapprezzoNumeroPostiOccupati1; }
        }

        public int SovrapprezzoNumeroPostiOccupati2
        {
            get { return sovrapprezzoNumeroPostiOccupati2; }
        }

        public int SovrapprezzoNumeroPostiOccupati3
        {
            get { return sovrapprezzoNumeroPostiOccupati3; }
        }

        public int SovrapprezzoBassaStagione
        {
            get { return sovrapprezzoBassaStagione; }
        }

        public int SovrapprezzoMediaStagione
        {
            get { return sovrapprezzoMediaStagione; }
        }

        public int SovrapprezzoAltaStagione
        {
            get { return sovrapprezzoAltaStagione; }
        }

        public DateTime InizioBassaStagione
        {
            get { return inizioBassaStagione; }
        }

        public DateTime FineBassaStagione
        {
            get { return fineBassaStagione; }
        }

        public DateTime InizioMediaStagione
        {
            get { return inizioMediaStagione; }
        }

        public DateTime FineMediaStagione
        {
            get { return fineMediaStagione; }
        }

        public DateTime InizioAltaStagione
        {
            get { return inizioAltaStagione; }
        }

        public DateTime FineAltaStagione
        {
            get { return fineAltaStagione; }
        }

    }
}
