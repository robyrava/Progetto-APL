using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using Classi;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerCancellaPrenotazione(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            (bool ok, Prenotazione? p) = DeserializzaConvalidaPrenotazione(request, conf);
            if (!ok)
            {
                RestituisciRispostaClient(false, "[Effettua check-in] Dati non validi", response);
            }
            else // deserializzazione e validazione andate a buon fine
            {
                try
                {
                    // connessione al DB
                    MySqlConnection connection = new MySqlConnection(conf.Constring);
                    connection.Open();
                    try
                    {
                        //Console.WriteLine("Connessione al DB riuscita.\nStato della connessione al DB: " + connection.State + "\n");
                        string messaggio;

                        // verifico che prenotazione non esista & check-in NON sia stato ancora effettuato
                        (bool prenotazioneValida, bool? cartaImbarcoTrovata, p.CodiceVolo, p.DataPartenza) = VerificaPrenotazione(connection, response, p.CodicePrenotazione, p.Documento);

                        if (!prenotazioneValida)
                        {
                            messaggio = "[Cancella prenotazione]: La prenotazione indicata non è stata trovata oppure è una prenotazione passata";
                            RestituisciRispostaClient(prenotazioneValida, messaggio, response);
                        }
                        else if (prenotazioneValida == true && cartaImbarcoTrovata == true)
                        {
                            messaggio = "Cancella prenotazione]: Il check-in è stato già effettuato.";
                            RestituisciRispostaClient(false, messaggio, response);
                        }
                        else if (prenotazioneValida == true && cartaImbarcoTrovata == false) // prenotazione esiste & check-in NON ancora effettuato
                        {
                            Console.WriteLine("[Cancella prenotazione]: Check-in NON ancora effettuato. Pertanto è possibile procedere con la cancellazione.");

                            // verifico se si rientri o meno nella finestra valida per effettuare la cancellazione: dal momento dell'acquisto fino a 2 giorni prima
                            DateTime finePeriodoValido = CalcolaData(p.DataPartenza, conf.FineCancellazione);

                            if(DateTime.Today.Date > finePeriodoValido)
                            {
                                RestituisciRispostaClient(ok, "Non è più possibile cancellare la prenotazione", response);
                            }
                            else // elimino prenotazione
                            {
                                Console.WriteLine("[Cancella prenotazione] Puoi procedere con la cancellazione");

                                ok = InterrogazioniDB.EliminaPrenotazione(connection, p.CodicePrenotazione, p.Documento);

                                if (!ok)
                                {
                                    messaggio = "[Cancella prenotazione]: Operazione NON completata";
                                    RestituisciRispostaClient(ok, messaggio, response);

                                }
                                else // elimino passeggero
                                {
                                    ok = InterrogazioniDB.EliminaPasseggero(connection, p.CodicePrenotazione, p.Documento);

                                    if (!ok)
                                    {
                                        messaggio = "[Cancella prenotazione]: Operazione NON completata";
                                        RestituisciRispostaClient(ok, messaggio, response);
                                    }
                                    else // modifico ricorrenza
                                    {
                                        ok = InterrogazioniDB.ModificaRicorrenzaNumeroPostiOccupati(connection, p.CodiceVolo, p.DataPartenza, -1);
                                        
                                        // chiusura connessione DB
                                        connection.Close();

                                        if (!ok)
                                        {
                                            messaggio = "[Cancella prenotazione]: Operazione NON completata";
                                            RestituisciRispostaClient(ok, messaggio, response);
                                        }
                                        else
                                        {
                                            messaggio = "[Cancella prenotazione]: Operazione completata";
                                            RestituisciRispostaClient(ok, messaggio, response);
                                        }
                                    }
                                }
                            }
                        }
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