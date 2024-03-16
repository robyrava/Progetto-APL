using MySql.Data.MySqlClient;
using Funzionalità;
using Classi;
using System.Net;
using System.Text.Json;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerCheckIn(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            (bool ok, Prenotazione? p) = DeserializzaConvalidaPrenotazione(request, conf);
            if (!ok)
            {
                RestituisciRispostaClient(false, "[Effettua check-in] Dati non validi", response);
            }
            else // validazione andata a buon fine
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
                            messaggio = "[Effettua check-in]: La prenotazione indicata non è stata trovata oppure è una prenotazione passata";
                            RestituisciRispostaClient(prenotazioneValida, messaggio, response);
                        }
                        else if (prenotazioneValida == true && cartaImbarcoTrovata == true)
                        {
                            messaggio = "[Effettua check-in]: Il check-in è stato già effettuato.";
                            RestituisciRispostaClient(false, messaggio, response);
                        }
                        else if (prenotazioneValida == true && cartaImbarcoTrovata == false) 
                        {
                            // prenotazione esiste & check-in NON è ancora stato effettuato

                            // verifico se si rientri o meno nella finestra valida per effettuare il check-in (da 2 gg a 1 giorno prima della data di partenza)
                            (ok, messaggio) = VerificaTempisticaCheckIn(conf, p.DataPartenza);

                            if (ok == false)
                            {
                                RestituisciRispostaClient(ok, messaggio, response);
                            }
                            else
                            {
                                Console.WriteLine(messaggio);

                                // recupero elenco dei posti a sedere, per la ricorrenza in questione
                                (ok, string? posti) = InterrogazioniDB.SelezionaPosti(connection, p.CodiceVolo, p.DataPartenza);

                                if (!ok)
                                {
                                    messaggio = "[Effettua check-in]: Operazione NON completata";
                                    RestituisciRispostaClient(ok, messaggio, response);

                                }
                                else
                                {
                                    // converto il JSON in un JsonElement
                                    JsonElement je = JsonSerializer.Deserialize<JsonElement>(posti);
                                    string? posto = DeterminaPostoCheckIn(conf.NumeroPostiRicorrenza, je);

                                    if (posto != null) // se è stato determinato correttamente un posto da assegnare...
                                    {
                                        (ok, string? codiceBiglietto) = InterrogazioniDB.InserisciCartaImbarco(connection, p, posto);

                                        if (!ok)
                                        {
                                            messaggio = "[Effettua check-in]: Operazione NON completata";
                                            RestituisciRispostaClient(ok, messaggio, response);

                                        }
                                        else
                                        {
                                            // aggiorno l'elenco dei posti a sedere (con il nuovo posto appena occupato)
                                            ok = InterrogazioniDB.ModificaRicorrenzaPosti(connection, p.CodiceVolo, p.DataPartenza, posto, "true");
                                            
                                            // chiusura connessione DB
                                            connection.Close();

                                            if (ok)
                                            {
                                                // creo un oggetto di classe Dictionary contenente i dati della carta d'imbarco
                                                Dictionary<string, object> cartaImbarco = GeneraCartaImbarco(p.CodicePrenotazione, codiceBiglietto, p.CodiceVolo, p.Documento, posto, null, null, default);
                                                StampaDizionario(cartaImbarco); // stampo in console carta d'imbarco
                                                
                                                response.ContentType = "application/json";
                                                string jsonCartaImbarco = JsonSerializer.Serialize(cartaImbarco);

                                                RestituisciRispostaClient(true, jsonCartaImbarco, response);
                                            }
                                            else
                                            {
                                                messaggio = "[Effettua check-in]: Operazione NON completata";
                                                RestituisciRispostaClient(ok, messaggio, response);
                                            }
                                        }
                                    }
                                    else // se si è verificato un problema in metodo DeterminaPosto...
                                    {
                                        messaggio = "[Effettua check-in]: Operazione NON completata";
                                        RestituisciRispostaClient(ok, messaggio, response);
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