using MySql.Data.MySqlClient;
using Funzionalità;
using Classi;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerModificaBagaglio(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Prenotazione? datiPrenotazione;
            bool esito;
            (esito, datiPrenotazione) = DeserializzaConvalidaPrenotazione(request, conf);

            if (!esito || datiPrenotazione == null)
            {
                // Invio una risposta al client
                RestituisciRispostaClient(false, "Dati non validi", response);
            }
            else
            {
                // Verifico che esite la prenotazione e che non sia scaduta
                using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                {
                    try
                    {
                        connection.Open();
                        (esito, MySqlDataReader? dataReader) = InterrogazioniDB.CercaPrenotazione(connection, datiPrenotazione.CodicePrenotazione, datiPrenotazione.Documento);
                        
                        //Chiudo il dataReader per evitare errori nelle richieste successive
                        if (dataReader != null)
                            dataReader.Close();

                        if (esito)
                        {

                            esito = InterrogazioniDB.GestioneBagaglio(connection, datiPrenotazione.CodicePrenotazione, datiPrenotazione.Documento);

                            if (esito) //se il bagaglio non è presente lo inserisco
                            {
                                esito = InterrogazioniDB.GestioneBagaglio(connection, datiPrenotazione.CodicePrenotazione, datiPrenotazione.Documento, true);
                                RestituisciRispostaClient(true, "Bagaglio inserito con successo", response);
                            }
                            else
                            {
                                RestituisciRispostaClient(false, "Errore: bagaglio già presente", response);
                            }

                        }
                        else
                        {
                            RestituisciRispostaClient(false, "Prenotazione non trovata o scaduta", response);
                        }
                    }
                    catch (MySqlException)
                    {
                        RestituisciRispostaClient(false, "Errore durante l'elaborazione della richiesta", response);
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