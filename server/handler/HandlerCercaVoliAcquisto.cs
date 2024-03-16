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

        // metodo per ricerca voli a partire da: data partenza, iata partenza, [iata arrivo]
        public static void HandlerCercaVoliAcquistoDataIata(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Ricorrenza? r = null;

            // leggo dati dal corpo della richiesta
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                try
                {
                    // si deserializzano i dati JSON in un oggetto di classe Ricorrenza
                    r = JsonSerializer.Deserialize<Ricorrenza>(reader.ReadToEnd(), conf.Options);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Errore durante la deserializzazione: {ex.Message}");
                }
            }

            // validazione campi
            if (!VerificaIata(r.IataAeroportoPartenza, r.IataAeroportoArrivo) || !ConfrontaData(r.DataPartenza, default, true))
            {
                string messaggio = "[Ricerca voli: data partenza, iata] Dati non validi.";
                RestituisciRispostaClient(false, messaggio, response);
            }
            else // validazione andata a buon fine
            {
                try
                {
                    // connessione al DB
                    MySqlConnection connection = new MySqlConnection(conf.Constring);
                    connection.Open();

                    try
                    {
                        //Console.WriteLine("Connessione al DB riuscita.\nStato della connessione al DB: " + connection.State + "\n");
                        MySqlDataReader? risultatoQuery = InterrogazioniDB.CercaVoliDataIata(conf, connection, r);

                        if (risultatoQuery != null) // almeno una ricorrenza presente in db corrisponde ai parametri di ricerca
                        {
                            // inserisco voli recuperati dal DB in una lista di oggetti di classe Dictionary
                            List<Dictionary<string, object>> risultati = OttieniElencoVoli(conf, risultatoQuery, true);

                            // stampo in console elenco di voli recuperati
                            foreach (Dictionary<string, object> risultatoD in risultati)
                            {
                                StampaDizionario(risultatoD);
                            }

                            response.ContentType = "application/json";
                            string jsonVoli = JsonSerializer.Serialize(risultati);

                            RestituisciRispostaClient(true, jsonVoli, response);
                        }
                        else // nessun dato recuperato <=> MySqlDataReader non contiene dati
                        {
                            string messaggio = "Non è stato trovato nessun volo corrispondente ai parametri di ricerca specificati.";
                            RestituisciRispostaClient(false, messaggio, response);
                        }

                        // chiusura connessione DB
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}