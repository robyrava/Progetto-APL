using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using Classi;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerInserisciRicorrenza(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            int rows;
            Ricorrenza? ricorrenza;
            ricorrenza = DeserializzaDati<Ricorrenza>(request, conf.Options);

            // Verifico i campi
            if (ricorrenza == null || !ConfrontaData(ricorrenza.DataPartenza, null, true) || !ConfrontaData(ricorrenza.DataArrivo, ricorrenza.DataPartenza, true) || !VerificaCodiceVolo(ricorrenza.CodiceVolo))
            {
                // Invio una risposta al client
                RestituisciRispostaClient(false, "Data o codice volo non validi", response);
            }
            else
            {
                // Creo una connessione al database e apro la connessione
                using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                {
                    try
                    {
                        connection.Open();
                        rows = InterrogazioniDB.InserisciRicorrenza(connection, ricorrenza);
                        
                        if (rows > 0)
                        {
                            RestituisciRispostaClient(true, "Ricorrenza inserita con successo!", response);
                        }
                        else
                        {
                            RestituisciRispostaClient(false, "Codice volo non trovato o ricorrenza già presente", response);
                        }                        
                    }
                    catch (MySqlException)
                    {
                        RestituisciRispostaClient(false, "Errore durante l'inserimento della ricorrenza", response);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                            
            }
        }
    }
}