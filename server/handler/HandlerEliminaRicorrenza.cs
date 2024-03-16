using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using Classi;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerEliminaRicorrenza(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Ricorrenza? ricorrenza;
            ricorrenza = DeserializzaDati<Ricorrenza>(request, conf.Options);

            //Verifico campi
            if (ricorrenza == null || !ConfrontaData(ricorrenza.DataArrivo, ricorrenza.DataPartenza, true) || !VerificaCodiceVolo(ricorrenza.CodiceVolo))
            {
                // Invia una risposta al client
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
                        
                        int rows = InterrogazioniDB.EliminaRicorrenza(connection, ricorrenza);
                        
                        if (rows > 0)
                        {
                            RestituisciRispostaClient(true, "Ricorrenza eliminata con successo!", response);
                        }
                        else
                        {
                            RestituisciRispostaClient(false, "Codice volo o data non trovati", response);
                        }
                    }
                    catch (MySqlException)
                    {
                        RestituisciRispostaClient(false, "Errore durante l'eliminazione della ricorrenza", response);
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