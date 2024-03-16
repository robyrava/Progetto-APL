using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerEliminaVolo(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            string? codiceVolo;
            codiceVolo = DeserializzaDati<string>(request, conf.Options);

            //Validazione codice volo
            if (codiceVolo == null || !VerificaCodiceVolo(codiceVolo))
            {
                // Invio una risposta al client python
                RestituisciRispostaClient(false, "Errore validazione campi", response);
            }
            else
            {
                // Creo una connessione al database e apro la connessione
                using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                {
                    try
                    {
                        connection.Open();
                        int rows = InterrogazioniDB.EliminaVolo(connection, codiceVolo);

                        if (rows > 0)
                        {
                            RestituisciRispostaClient(true, "Volo e relative ricorrenze eliminate con successo!", response);
                        }
                        else
                        {
                            RestituisciRispostaClient(false, "Codice volo non trovato", response);
                        }
                    }
                    catch (MySqlException ex)
                    {
                        RestituisciRispostaClient(false, "Errore durante l'eliminazione del volo: " + ex.Message, response);
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