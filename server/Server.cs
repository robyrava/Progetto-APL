using System.Net;
using Funzionalità;
using static AirlineManagerServer.Handler;


namespace AirlineManagerServer
{
    
    class Server
    {
        //lista dei client connessi
        static List<HttpListenerRequest> listConnectedClients = new List<HttpListenerRequest>();
        static int Main(string[] args)
        {
            Configurazione conf = new Configurazione();
            HttpListener listener = AvviaServer(conf);
            
            while (true)
            {
                Console.WriteLine("\nServer in ascolto...\n");

                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // Crea un nuovo thread per ogni richiesta
                Thread thread = new Thread(() => HandleRequest(conf, request, response));
                thread.Start(); // Avvia il thread
                
                // Aggiungi il client alla lista
                listConnectedClients.Add(request);
                //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
            }
        }

        static void HandleRequest(Configurazione conf, HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                //GV----------------------------------------------------------------------------------------
                if (request.Url.AbsolutePath == conf.Path["pathCercaVoliDataIata"])
                {
                    HandlerCercaVoliAcquistoDataIata(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathInserisciPrenotazione"])
                {
                    HandlerInserisciPrenotazione(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathInserisciPasseggero"])
                {
                    HandlerInserisciPasseggero(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathModificaRicorrenzaNumeroPostiOccupati"])
                {
                    HandlerModificaRicorrenzaNumeroPostiOccupati(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathCheckIn"])
                {
                    HandlerCheckIn(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathCancellaPrenotazione"])
                {
                    HandlerCancellaPrenotazione(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathVisualizzaCartaImbarco"])
                {
                    HandlerVisualizzaCartaImbarco(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
//RR97--------------------------------------------------------------------------------------
                else if (request.Url.AbsolutePath == conf.Path["pathLoginAdmin"])
                {
                    HandlerLoginAdmin(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathInserisciVolo"])
                {
                    HandlerInserisciVolo(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathRicercaVolo"])
                {
                    HandlerRicercaVolo(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathEliminaVolo"])
                {
                    HandlerEliminaVolo(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathInserisciRicorrenza"])
                {
                    HandlerInserisciRicorrenza(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathEliminaRicorrenza"])
                {
                    HandlerEliminaRicorrenza(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if (request.Url.AbsolutePath == conf.Path["pathModificaRicorrenza"])
                {
                    HandlerModificaRicorrenza(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if(request.Url.AbsolutePath == conf.Path["pathModificaBagaglio"])
                {
                    HandlerModificaBagaglio(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if(request.Url.AbsolutePath == conf.Path["pathRichiestaPosto"])
                {
                    HandlerRichiestaPosto(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }
                else if(request.Url.AbsolutePath == conf.Path["pathAssegnaPosto"])
                {
                    HandlerAssegnaPosto(conf, request, response);
                    listConnectedClients.Remove(request); // Rimuovi il client dalla lista
                    //Console.WriteLine("\nNumero di client attivi: {0}\n", listConnectedClients.Count);
                    response.Close();
                }                
//--------------------------------------------------------------------------------------
                else
                {
                    Console.WriteLine("Richiesta HTTP non gestita dal server.");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }

            }
            catch (Exception)
            {
                // Il client si è disconnesso
                Console.WriteLine("Il client {0} si è disconnesso", request.RemoteEndPoint);
                listConnectedClients.Remove(request); // Rimuovi il client dalla lista
            }
        }

        private static HttpListener AvviaServer(Configurazione conf)
        {
            HttpListener listener = new HttpListener();
    
            //loop su url list = urlList
            foreach (string url in conf.UrlList)
            {
                listener.Prefixes.Add(url);
            }

            listener.Start();

            return listener;
        }
    }
}
