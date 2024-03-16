using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerModificaRicorrenza(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Dictionary<string, string>? dati;
            dati = DeserializzaDati<Dictionary<string, string>>(request, conf.Options);
            
            // Verifico i valori ricevuti
            if (dati == null || !ConfrontaData(dati["dataPartenza"], null, true) || !ConfrontaData(dati["dataArrivo"], dati["dataPartenza"], true) || !VerificaCodiceVolo(dati["codiceVolo"]) || !ConfrontaData(dati["nuovaDataPartenza"], null, true) || !ConfrontaData(dati["nuovaDataArrivo"], dati["nuovaDataPartenza"], true))
            {
                // Invio una risposta al client
                RestituisciRispostaClient(false, "Date o codice volo non validi", response);
            }
            else
            {
                // Creo una connessione al database e apro la connessione
                using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                {
                    try
                    {
                        connection.Open();
                        int rows = InterrogazioniDB.ModificaRicorrenza(connection, dati);
                        // Invio una risposta al client
                        if (rows > 0)
                        {
                            RestituisciRispostaClient(true, "Ricorrenza modificata con successo!", response);
                        }
                        else
                        {
                            RestituisciRispostaClient(false, "Ricorrenza non trovata", response);
                        }
                    }
                    catch (MySqlException)
                    {
                        RestituisciRispostaClient(false, "Si è verificato un errore in DB durante la modifica della ricorrenza", response);
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