using MySql.Data.MySqlClient;
using Funzionalità;
using static Funzionalità.Validator;
using Classi;
using System.Net;


namespace AirlineManagerServer
{
    partial class Handler
    {
        public static void HandlerLoginAdmin(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            Admin? admin;
            admin = DeserializzaDati<Admin>(request, conf.Options);

            //Validazione campi
            if (admin == null || !VerificaLoginAdmin(admin))
            {
                // Invio una risposta al client
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

                        bool login = InterrogazioniDB.LoginAdmin(connection, admin);

                        if (login)
                        {
                            RestituisciRispostaClient(true, "Login effettuato!", response);
                        }
                        else
                        {
                            RestituisciRispostaClient(false, "Username o password errati", response);
                        }
                    }
                    catch (MySqlException)
                    {
                        RestituisciRispostaClient(false, "Errore di connessione al database", response);
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