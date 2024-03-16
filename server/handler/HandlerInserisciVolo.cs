using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using Classi;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerInserisciVolo(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Volo? volo;
            volo = DeserializzaDati<Volo>(request, conf.Options);

            //Validazione campi
            if (volo == null || !VerificaVoloCompleto(volo))
            {
                // Invia una risposta al client
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
                        int rows = InterrogazioniDB.InserisciVolo(connection, volo);
                        
                        // Invio una risposta al client python
                        if (rows > 0)
                        {
                            RestituisciRispostaClient(true, "Volo inserito", response);
                        }
                        else
                        {
                            RestituisciRispostaClient(false, "Impossibile inserire il volo", response);
                        }
                    }
                    catch (MySqlException)
                    {
                        RestituisciRispostaClient(false, "Errore durante l'inserimento del volo", response);
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

