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

        public static void HandlerInserisciPasseggero(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Passeggero? p = null;

            // leggo dati dal corpo della richiesta
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                try
                {
                    // si deserializzano i dati JSON in un oggetto di classe Passeggero
                    p = JsonSerializer.Deserialize<Passeggero>(reader.ReadToEnd(), conf.Options);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Errore durante la deserializzazione: {ex.Message}");
                }
            }

            // validazione campi
            if (!VerificaDocumento(p.Documento) || !VerificaCodicePrenotazione(p.CodicePrenotazione.Replace("\r\n", "")) || !VerificaCodiceFiscale(p.CodiceFiscale) || !VerificaNomeCognome(p.Nome) || !VerificaNomeCognome(p.Cognome) || !ConfrontaData(p.DataNascita) || !VerificaTelefono(p.Telefono))
            {
                string messaggio = "[Creazione nuovo passeggero] Dati non validi";
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
                        bool passeggeroCreato = InterrogazioniDB.InserisciPasseggero(connection, p);
                        
                        // chiusura connessione DB
                        connection.Close();

                        if (passeggeroCreato == true) // dati nuovo passeggero inseriti in DB
                        {
                            string messaggio = "[Creazione nuovo passeggero]: Operazione completata";
                            RestituisciRispostaClient(passeggeroCreato, messaggio, response);
                        }
                        else
                        {
                            string errore = "[Creazione nuovo passeggero]: Operazione NON completata";
                            RestituisciRispostaClient(passeggeroCreato, errore, response);
                        }
                        response.Close();
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