using MySql.Data.MySqlClient;
using Funzionalità;
using Classi;
using System.Net;
using System.Text.Json;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerVisualizzaCartaImbarco(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            (bool ok, Prenotazione? p) = DeserializzaConvalidaPrenotazione(request, conf);
            if (!ok)
            {
                RestituisciRispostaClient(false, "[Visualizza carta d'imbarco] Dati non validi", response);
            }
            else // validazione andata a buon fine
            {
                // accerto il fatto che il check-in sia già stato effettuato
                (ok, string messaggio, p.CodiceVolo, p.DataPartenza) = VerificaCheckIn(conf, p);

                if (!ok)
                {
                    // Invia una risposta al client
                    RestituisciRispostaClient(false, messaggio, response);
                }
                else
                {
                    MySqlConnection connection = new MySqlConnection(conf.Constring);
                    connection.Open();
                    try
                    {
                        MySqlDataReader? risultatoQuery = InterrogazioniDB.RecuperaDatiCartaImbarco(connection, p.CodicePrenotazione);

                        if (risultatoQuery != null)
                        {
                            risultatoQuery.Read();

                            string codiceBiglietto = risultatoQuery["codiceBiglietto"].ToString();
                            string posto = risultatoQuery["posto"].ToString();
                            int bagaglio = (int)risultatoQuery["bagaglio"];
                            risultatoQuery.Close();

                            // dizionario con i dati da inviare al client come risposta
                            Dictionary<string, object> infoCartaImbarco = GeneraCartaImbarco(p.CodicePrenotazione, codiceBiglietto, p.CodiceVolo, p.Documento, posto, p.DataPartenza, bagaglio, true);
                            StampaDizionario(infoCartaImbarco); // stampo in console dettagli su carta d'imbarco

                            response.ContentType = "application/json";
                            string jsonInfoCartaImbarco = JsonSerializer.Serialize(infoCartaImbarco);

                            RestituisciRispostaClient(true, jsonInfoCartaImbarco, response);
                        }
                        else // nessun dato recuperato <=> MySqlDataReader non contiene dati
                        {
                            RestituisciRispostaClient(false, "[Visualizza carta d'imbarco] Errore", response);
                        }

                        // chiusura connessione DB
                        connection.Close();
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}