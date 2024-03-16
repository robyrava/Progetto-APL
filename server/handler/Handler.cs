using System.Net;
using System.Text.Json;
using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using Classi;


namespace AirlineManagerServer
{
    partial class Handler
    {

        
//GV & RR97--------------------------------------------------------------------------------------------------------------
        // utente: acquisto = true; admin: acquisto = false 
        static List<Dictionary<string, object>> OttieniElencoVoli(Configurazione conf, MySqlDataReader risultatoQuery, bool acquisto = false)
        {
            List<Dictionary<string, object>> risultati = new List<Dictionary<string, object>>();
            Dictionary<string, object> risultato;
            int sovrapprezzoTotale;

            while (risultatoQuery.Read())
            {
                risultato = new Dictionary<string, object>();

                if (acquisto)
                {
                    // calcolo sovrapprezzi
                    sovrapprezzoTotale = DeterminaSovrapprezzoNumeroPostiOccupati(conf, (int)risultatoQuery["numeroPostiOccupati"]);
                    sovrapprezzoTotale += DeterminaSovrapprezzoStagione(conf, (DateTime)risultatoQuery["dataPartenza"]);
                }
                else sovrapprezzoTotale = 0; // acquisto = false (default)

                for (int i = 0; i < risultatoQuery.FieldCount; i++) // per ogni campo della tupla corrente...
                {
                    //Console.WriteLine(data_reader.GetName(i));
                    if (risultatoQuery.GetName(i) == "prezzoBase" && sovrapprezzoTotale > 0)
                    {
                        risultato.Add(risultatoQuery.GetName(i), (float)risultatoQuery.GetValue(i) + sovrapprezzoTotale);
                    }
                    else
                    {
                        risultato.Add(risultatoQuery.GetName(i), risultatoQuery.GetValue(i));
                    }
                }
                risultati.Add(risultato);
            }
            risultatoQuery.Close();

            return risultati;
        }

//GV---------------------------------------------------------------------------------------------------------------

        // genera un sovrapprezzo in funzione del range all'interno del quale si trova il parametro numeroPostiOccupati
        static int DeterminaSovrapprezzoNumeroPostiOccupati(Configurazione conf, int numeroPostiOccupati)
        {
            // calcolo sovrapprezzo sulla base di numero_posti_occupati

            // per i posti da 1 a 10 il sovrapprezzo è pari a 0  
            if (numeroPostiOccupati >= 11 && numeroPostiOccupati <= 30)
            {
                return conf.SovrapprezzoNumeroPostiOccupati1;
            }
            else if (numeroPostiOccupati >= 31 && numeroPostiOccupati <= 60) // compreso fra 31 e 60
            {
                return conf.SovrapprezzoNumeroPostiOccupati2;
            }
            else if (numeroPostiOccupati >= 61 && numeroPostiOccupati <= 89) // compreso fra 61 e 90
            {
                return conf.SovrapprezzoNumeroPostiOccupati3;
            }
            else
            {
                return 0;
            }
        }


        // genera un sovrapprezzo in funzione della stagione (bassa/media/alta) entro cui ricade la ricorrenza considerata
        static int DeterminaSovrapprezzoStagione(Configurazione conf, DateTime dataPartenza)
        {
            // calcolo sovrapprezzo, in base al fatto che la partenza sia prevista in bassa/media/alta stagione

            // se la partenza è in BASSA stagione  
            if (dataPartenza >= conf.InizioBassaStagione &&  dataPartenza <= conf.FineBassaStagione)
            {
                return conf.SovrapprezzoBassaStagione;
            }
            else if (dataPartenza >= conf.InizioMediaStagione && dataPartenza <= conf.FineMediaStagione) // se la partenza è in MEDIA stagione
            {
                return conf.SovrapprezzoMediaStagione;
            }
            else // se la partenza è in ALTA stagione
            {
                return conf.SovrapprezzoAltaStagione;
            }
        }
        

        // realizza deserializzazione e validazione dei dati di prenotazione provenienti dal client 
        static (bool ok, Prenotazione? p) DeserializzaConvalidaPrenotazione(HttpListenerRequest request, Configurazione conf)
        {
            Prenotazione? p = null;

            // leggo dati dal corpo della richiesta
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                try
                {
                    // si deserializzano i dati JSON in un oggetto di classe Prenotazione
                    p = JsonSerializer.Deserialize<Prenotazione>(reader.ReadToEnd(), conf.Options);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Errore durante la deserializzazione: {ex.Message}");
                }
            }

            // validazione campi
            if (!VerificaCodicePrenotazione(p.CodicePrenotazione) || !VerificaDocumento(p.Documento))
            {
                return (false, null);
            }

            return (true, p);
        }


        // verifica se si rientri nelle tempistiche valide per effettuare il check-in
        static (bool ok, string messaggio) VerificaTempisticaCheckIn(Configurazione conf, string dataPartenza)
        {
            string messaggio;
            DateTime dataOdierna = DateTime.Today.Date;

            // determino estremi intervallo utile per effettuare check-in
            DateTime inizioPeriodoValido = CalcolaData(dataPartenza, conf.InizioCheckIn);
            DateTime finePeriodoValido = CalcolaData(dataPartenza, conf.FineCheckIn);

            if(dataOdierna < inizioPeriodoValido)
            {
                messaggio = "[Effettua check-in] Check-in NON ancora disponibile. Sarà possibile effettuarlo a partire da 2 giorni prima rispetto alla data di partenza";
                return (false, messaggio);
            }
            else if(dataOdierna > finePeriodoValido)
            {
                messaggio = "[Effettua check-in] NON è più possibile effettuare il check-in";
                return (false, "");
            }

            return (true, "[Effettua check-in] Puoi procedere col check-in");
        }


        // a partire dal json recuperato dal db, effettua la ricerca di un posto a sedere da assegnare
        static string? DeterminaPostoCheckIn(int numeroPostiRicorrenza, JsonElement je)
        {
            foreach (JsonProperty property in je.EnumerateObject())
            {
                if (property.Value.ValueEquals("false"))
                {
                    return property.Name;
                }
            }

            return null;
        }


        // verifica che la prenotazione indicata sia valida (esiste ed è ancora attiva) e che NON sia ancora stato effettuato il check-in
        static (bool, bool?, string?, string?) VerificaPrenotazione(MySqlConnection connection, HttpListenerResponse response, string codicePrenotazione, string documento)
        {
            string messaggio;
            (bool prenotazioneValida, MySqlDataReader? risultati) = InterrogazioniDB.CercaPrenotazione(connection, codicePrenotazione, documento);

            if (!prenotazioneValida) // prenotazione non trovata oppure scaduta
            {
                return (prenotazioneValida, null, null, null);

            }
            else // se prenotazione è valida (cioè esiste ed è ancora attiva)
            {
                risultati.Read();
                // seleziono dal data reader solo i dati di interesse
                string codiceVolo = risultati["codiceVolo"].ToString();
                string dataPartenza = DateTime.Parse(risultati["dataPartenza"].ToString()).ToString("yyyy-MM-dd");
                risultati.Close();

                bool cartaImbarcoTrovata = InterrogazioniDB.CercaCartaImbarco(connection, codicePrenotazione);

                if (!cartaImbarcoTrovata) // prenotazione valida && check-in non ancora effettuato
                {
                    return (prenotazioneValida, cartaImbarcoTrovata, codiceVolo, dataPartenza);

                }

                // prenotazione valida && check-in già effettuato
                return (prenotazioneValida, cartaImbarcoTrovata, null, null);
            }
        }


        // input: data in formato stringa ; output: data in formato DateTime, con giorni aggiunti/sottratti
        static DateTime CalcolaData(string data, short valore)
        {
            return DateTime.Parse(data).AddDays(valore);
        }


        // check-in: visualizza = false 
        static Dictionary<string, object> GeneraCartaImbarco(string codicePrenotazione, string codiceBiglietto, string codiceVolo, string documento, string posto, string? dataPartenza, int? bagaglio, bool visualizza = false)
        {
            Dictionary<string, object> cartaImbarco = new Dictionary<string, object>
            {
                { "codicePrenotazione", codicePrenotazione },
                { "codiceBiglietto", codiceBiglietto },
                { "codiceVolo", codiceVolo },
                { "documento", documento },
                { "postoAssegnato", posto }
            };

            if(visualizza) {
                cartaImbarco.Add("dataPartenza", dataPartenza);
                cartaImbarco.Add("bagaglio", bagaglio);
            }

            return cartaImbarco;
        }


        // definisce la risposta da inviare al client, anche in funzione dell'esito positivo o negativo della richiesta
        static void RestituisciRispostaClient(bool flag, string risposta, HttpListenerResponse response)
        {
            if (flag == true)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            
            Console.WriteLine("Invio risposta al client:\n" + risposta);
            
            using (StreamWriter writer = new StreamWriter(response.OutputStream))
            {
                writer.WriteLine(risposta);
            }
        }

//RR97--------------------------------------------------------------------------------------------------------------
        
        public static T? DeserializzaDati<T>(HttpListenerRequest request, JsonSerializerOptions options)
        {
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                try
                {
                    // Deserializzo i dati JSON in un oggetto del tipo specificato
                    return JsonSerializer.Deserialize<T>(reader.ReadToEnd(), options);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine("Errore durante la deserializzazione dei dati JSON: " + ex.Message);
                    return default;
                }
            }
        }

        static void StampaDizionario(Dictionary<string, object> dizionario)
        {
            // stampo in console items
            foreach (KeyValuePair<string, object> kv in dizionario)
            {
                Console.WriteLine(kv.Key + ": " + kv.Value);
            }
        }
        static (bool, string?, string?) RecuperaRicorrenza(Configurazione conf, string codicePrenotazione, string documento)
        {
            bool esito;
            string? codiceVolo = null, dataPartenza = null;

            using (MySqlConnection connection = new MySqlConnection(conf.Constring))
            {
                connection.Open();
                (esito, MySqlDataReader reader) = InterrogazioniDB.CercaPrenotazione(connection, codicePrenotazione, documento);
                
                if (esito)
                {
                    //recupero codice volo e data partenza
                    reader.Read();
                    codiceVolo = reader["codiceVolo"].ToString();
                    dataPartenza = DateTime.Parse(reader["dataPartenza"].ToString()).ToString("yyyy-MM-dd"); 
                    reader.Close();
                }

                connection.Close();
            } 

            return (esito, codiceVolo, dataPartenza);
        }
        
        //metodo che verifica se esite la prenotazione e se è stato effettuato il check-in
        static (bool, string, string?, string?) VerificaCheckIn(Configurazione conf, Prenotazione datiPrenotazione)
        {
            bool esito;
            string? dataPartenza = null, codiceVolo = null;

            //Verifico prenotazione
            using (MySqlConnection connection = new MySqlConnection(conf.Constring))
            {
                try
                {
                    connection.Open();
                    (esito, MySqlDataReader reader) = InterrogazioniDB.CercaPrenotazione(connection, datiPrenotazione.CodicePrenotazione, datiPrenotazione.Documento);

                    if (esito)
                    {
                        //recupero codice volo e data partenza
                        reader.Read();
                        codiceVolo = reader["codiceVolo"].ToString();
                        dataPartenza = DateTime.Parse(reader["dataPartenza"].ToString()).ToString("yyyy-MM-dd");
                        reader.Close();

                        //verifico se è stato effettuato il check-in
                        esito = InterrogazioniDB.CercaCartaImbarco(connection, datiPrenotazione.CodicePrenotazione);
                        if (esito)
                        {
                            return (true, "ok", codiceVolo, dataPartenza);
                        }
                        else
                        {
                            return (false, "Errore: check-In non ancora effettuato!", null, null);
                        }
                    }
                    else
                    {
                        return (false, "Errore: prenotazione non trovata o scaduta", null, null);
                    }
                }
                catch (MySqlException ex)
                {
                    return (false, "Errore durante la connessione al DB!", null, null);
                }
                finally
                {
                    connection.Close();
                }
            }            
        }    
//--------------------------------------------------------------------------------------------------------------
        
    }
}