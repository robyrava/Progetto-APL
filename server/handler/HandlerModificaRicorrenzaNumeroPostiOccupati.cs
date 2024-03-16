using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using Classi;
using System.Net;
using System.Text.Json;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerModificaRicorrenzaNumeroPostiOccupati(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
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
            if (!VerificaCodiceVolo(r.CodiceVolo) || !ConfrontaData(r.DataPartenza, default, true))
            {
                string messaggio = "[Aggiornamento ricorrenza] Dati non validi";
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
                        bool ricorrenzaAggiornata = InterrogazioniDB.ModificaRicorrenzaNumeroPostiOccupati(connection, r.CodiceVolo, r.DataPartenza, 1);
                        
                        // chiusura connessione DB
                        connection.Close();

                        if (ricorrenzaAggiornata == true) // ricorrenza aggiornata in DB
                        {
                            string messaggio = "[Aggiornamento ricorrenza] Operazione completata.";
                            RestituisciRispostaClient(ricorrenzaAggiornata, messaggio, response);
                        }
                        else
                        {
                            string errore = "[Aggiornamento ricorrenza] Operazione NON completata.";
                            RestituisciRispostaClient(ricorrenzaAggiornata, errore, response);
                        }
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