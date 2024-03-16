using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using System.Net;
using System.Text.Json;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerRicercaVolo(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            string? codiceVolo;
            codiceVolo = DeserializzaDati<string>(request, conf.Options);

            //Validazione campi
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

                        MySqlDataReader risultatoQuery = InterrogazioniDB.CercaVolo(connection, codiceVolo);

                        if (risultatoQuery.HasRows)
                        {
                            List<Dictionary<string, object>> listaVoli = new List<Dictionary<string, object>>();
                            listaVoli = OttieniElencoVoli(conf, risultatoQuery);
                            
                            // Invia una risposta al client
                            RestituisciRispostaClient(true, JsonSerializer.Serialize(listaVoli), response);
                        }
                        else
                        {
                            // Il DB non contiene dati
                            RestituisciRispostaClient(false, "Codice volo non trovato o non ancora schedulato", response);
                        }

                        //risultatoQuery.Close();
                    }
                    catch (MySqlException)
                    {
                        RestituisciRispostaClient(false, "Errore durante l'esecuzione della query", response);
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