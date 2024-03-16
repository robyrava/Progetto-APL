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

        public static void HandlerInserisciPrenotazione(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
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
            if (!(VerificaDocumento(p.Documento) && VerificaCodiceVolo(p.CodiceVolo) && VerificaImporto(p.Importo)))
            {
                string messaggio = "[Crea nuova prenotazione] Dati non validi";
                RestituisciRispostaClient(false, messaggio, response);
            }
            else // validazione andata a buon fine
            {
                try
                {
                    // connessione al DB
                    MySqlConnection connection = new MySqlConnection(conf.Constring);

                    // chiusura connessione al DB
                    connection.Open();
                    
                    try
                    {
                        //Console.WriteLine("Connessione al DB riuscita.\nStato della connessione al DB: " + connection.State + "\n");
                        (bool prenotazioneCreata, string? codicePrenotazione) = InterrogazioniDB.InserisciPrenotazione(connection, p);
                        
                        // chiusura connessione DB
                        connection.Close();

                        if (prenotazioneCreata == true) // dati nuova prenotazione inseriti in DB
                        {
                            RestituisciRispostaClient(prenotazioneCreata, codicePrenotazione, response);
                        }
                        else
                        {
                            string errore = "[Creazione nuova prenotazione]: Operazione NON completata";
                            RestituisciRispostaClient(prenotazioneCreata, errore, response);
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