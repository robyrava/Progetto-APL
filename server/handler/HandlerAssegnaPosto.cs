using MySql.Data.MySqlClient;
using Funzionalità;
using System.Net;


namespace AirlineManagerServer
{

    partial class Handler
    {

        public static void HandlerAssegnaPosto(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Dictionary<string, string>? dati;
            bool esito;
            string? codiceVolo, dataPartenza, postoVecchio;

            dati = DeserializzaDati<Dictionary<string, string>>(request, conf.Options);

            if (dati == null)
            {
                RestituisciRispostaClient(false, "Errore durante la deserializzazione dei dati", response);
                return;
            }
            
            (esito, codiceVolo, dataPartenza) = RecuperaRicorrenza(conf, dati["codicePrenotazione"], dati["documento"]);

            if (!esito || codiceVolo == null || dataPartenza == null)
                RestituisciRispostaClient(false, "Errore durante il recupero dei dati", response);
            else
            {
                //recupero posto vecchio
                using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                {
                    try
                    {
                        connection.Open();
                        postoVecchio = InterrogazioniDB.RecuperaPosto(connection, dati["codicePrenotazione"], dati["documento"]);
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Errore durante la connessione al Db : " + ex.Message);
                        postoVecchio = null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                if (postoVecchio == null)
                    RestituisciRispostaClient(false, "Errore durante il recupero del posto", response);
                else
                {
                    //libero vecchio posto
                    using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                    {
                        try
                        {
                            connection.Open();
                            esito = InterrogazioniDB.ModificaRicorrenzaPosti(connection, codiceVolo, dataPartenza, postoVecchio, "false");
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine("Errore durante la connessione al Db : " + ex.Message);
                            esito = false;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }

                    if (esito)
                    {
                        Console.WriteLine("Il posto precedente è stato liberato con successo!");

                        //assegno nuovo posto
                        using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                        {
                            try
                            {
                                connection.Open();
                                esito = InterrogazioniDB.ModificaRicorrenzaPosti(connection, codiceVolo, dataPartenza, dati["posto"], "true");
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine("Errore durante la connessione al Db : " + ex.Message);
                                esito = false;
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }

                    if (esito)
                    {
                        //Aggiorno posto carta imbarco
                        using (MySqlConnection connection = new MySqlConnection(conf.Constring))
                        {
                            try
                            {
                                connection.Open();
                                InterrogazioniDB.AggiornaPostoCartaImbarco(connection, dati["codicePrenotazione"], dati["documento"], dati["posto"]);
                                Console.WriteLine("Posto carta di imbarco aggiornato!\n");
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine("Errore durante l'aggiornamento del posto carta di imbarco: " + ex.Message);
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }

                        RestituisciRispostaClient(true, "Posto assegnato con successo!\n", response);
                    }
                    else
                        RestituisciRispostaClient(false, "Errore durante l'assegnazione del posto\n", response);
                }
            }
        }

    }

}