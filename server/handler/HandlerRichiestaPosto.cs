using MySql.Data.MySqlClient;
using Funzionalità;
using Classi;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerRichiestaPosto(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Prenotazione? datiPrenotazione;
            bool esito;
            string posti, messaggio, codiceVolo, dataPartenza;
            (esito, datiPrenotazione) = DeserializzaConvalidaPrenotazione(request, conf);

            if (!esito || datiPrenotazione == null)
            {
                // Invia una risposta al client
                RestituisciRispostaClient(false, "Dati non validi", response);
            }
            else
            {
                (esito, messaggio, codiceVolo, dataPartenza) = VerificaCheckIn(conf, datiPrenotazione);

                if (!esito)
                {
                    // Invia una risposta al client
                    RestituisciRispostaClient(false, messaggio, response);
                }
                else
                {
                    //Invio il dizionario dei posti
                    using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                    {
                        try
                        {
                            connection.Open();
                            (esito, posti) = InterrogazioniDB.SelezionaPosti(connection, codiceVolo, dataPartenza);
                            
                            if (esito)
                            {
                                RestituisciRispostaClient(true, posti, response);
                            }
                            else //Non verra mai restituito questo messaggio
                            {
                                RestituisciRispostaClient(false, "Errore: posti non trovati", response);
                            }
                        }
                        catch (MySqlException)
                        {
                            RestituisciRispostaClient(false, "Errore durante l'apertura della connessione", response);
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

}