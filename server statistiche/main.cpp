#include <iostream>
#include <mysql_driver.h> 
#include <mysql_connection.h> 
#include <cppconn/resultset.h>
#include <cppconn/statement.h>
#include "httplib.h"
#include "nlohmann/json.hpp"
#include "handler.h"

//porta del server
#define PORT 1234

using namespace std;

int main(void)
{
    //oggetto server
    httplib::Server svr;


    // Imposta un logger per stampare un messaggio ogni volta che un client si connette
    svr.set_logger([](const httplib::Request& req, const httplib::Response& res) {
        cout << "Il client con indirizzo: " << req.remote_addr << " e porta:" << req.remote_port << " si � connesso" << endl;
        });


    //RR97----------------------------------------------------------------------------------------------
    
    // Richieste POST PER STATISTICA 1
    svr.Post("/statistica/uno", [](const httplib::Request& req, httplib::Response& res) {

        handlerStatistica1(req, res);
    }
    );


    // Richieste POST PER STATISTICA 3
    svr.Post("/statistica/tre", [](const httplib::Request& req, httplib::Response& res) {

        handlerStatistica3(req, res);
    }
    );

    //----------------------------------------------------------------------------------------------

    // Richieste POST PER STATISTICA 2
    svr.Post("/statistica/due", [](const httplib::Request& req, httplib::Response& res) {

        handlerStatistica2(req, res);
    }
    );


    // Richieste POST PER STATISTICA 4
    svr.Post("/statistica/quattro", [](const httplib::Request& req, httplib::Response& res) {

        handlerStatistica4(req, res);
    }
    );


    // Avvia il server sulla porta definita
    cout << "Server in attesa di connessioni...\n" << endl;
    svr.listen("localhost", PORT);

    return 0;
}