#include <iostream>
#include <mysql_connection.h> 
#include <cppconn/resultset.h>
#include <cppconn/statement.h>
#include "httplib.h"
#include "nlohmann/json.hpp"
#include "handler.h"
#include "utilities.h"


using namespace std;
using json = nlohmann::json; //alias per la classe json



void handlerStatistica1(const httplib::Request& req, httplib::Response& res) {
    // Decodifico i dati dalla richiesta in un oggetto json
    json data = json::parse(req.body);

    // Estraggo i campi "codice_volo" e "anno" dal json
    string codice_volo = data["codice_volo"];
    string anno = data["anno"];

    // Creo una connessione al database MySQL
    sql::Connection* conn = creaConnessioneDB();

    // Scrivo la query SQL come una stringa
    string query = "SELECT MONTH(dataPartenza) AS mese, COUNT(*) AS voli "
        "FROM RICORRENZE "
        "WHERE codiceVolo = '" + codice_volo + "' AND YEAR(dataPartenza) = '" + anno + "' "
        "GROUP BY mese";

    // Eseguo la query e restituisco i risultati come stringa
    string response = eseguiQuery(conn, query);

    // Imposto il corpo della risposta con la stringa creata
    res.set_content(response, "text/plain");


    // delete conn
    libera_risorse(nullptr, conn);
}


void handlerStatistica2(const httplib::Request& req, httplib::Response& res) {
    cout << "[Statistica 2] Richiesta proveniente dal client" << endl;

    // Decodifica i dati dalla richiesta come un oggetto json
    json data = json::parse(req.body);

    // Estraggo il campo anno dal json
    string codiceVolo = data["codiceVolo"];

    //cout << anno << endl;

    // Crea una connessione al database mysql
    sql::Connection* conn = creaConnessioneDB();

    // Scrivi la query sql come una stringa
    string totalePosti = "90";

    string query = "SELECT dataPartenza, numeroPostiOccupati/" + totalePosti + "*100 AS percentuale "
        "FROM ricorrenze "
        "WHERE codiceVolo = '" + codiceVolo + "'"
        "ORDER BY percentuale";

    // Crea uno statement ed esegue la query, ottenendo un oggetto result
    sql::ResultSet* result = ottieni_risultato_query(conn, query);

    // Crea una stringa con i risultati separati da spazi e a capo
    string response;
    while (result->next()) {
        string d = result->getString("dataPartenza");
        string dataPartenza = d.substr(8, 2) + "-" + d.substr(5, 2) + "-" + d.substr(0, 4);

        response += (dataPartenza)+" " + to_string(result->getInt("percentuale")) + "\n";
    }

    // Imposta il corpo della risposta con la stringa creata
    res.set_content(response, "text/plain");

    // Libera le risorse
    libera_risorse(result, conn);
    //delete stmt;
}


void handlerStatistica3(const httplib::Request& req, httplib::Response& res) {
    // Decodifico i dati dalla richiesta in un oggetto json
    json data = json::parse(req.body);
    // Estraggo il campo "anno" dal json
    string anno = data["anno"];

    // Creo una connessione al database MySQL
    sql::Connection* conn = creaConnessioneDB();

    // Scrivo la query SQL come una stringa
    string query = "SELECT v.iataAeroportoPartenza, COUNT(*) AS NumeroRicorrenze "
        "FROM ricorrenze r "
        "JOIN voli v ON r.codiceVolo = v.codiceVolo "
        "WHERE YEAR(r.dataPartenza) = '" + anno + "' "
        "GROUP BY v.iataAeroportoPartenza";

    // Eseguo la query e restituisco i risultati come stringa
    string response = eseguiQuery(conn, query);

    // Imposto il corpo della risposta con la stringa creata
    res.set_content(response, "text/plain");

    // delete conn
    libera_risorse(nullptr, conn);
}


void handlerStatistica4(const httplib::Request& req, httplib::Response& res) {
    cout << "[Statistica 4] Richiesta proveniente dal client" << endl;

    // Decodifica i dati dalla richiesta come un oggetto json
    json data = json::parse(req.body);

    // Estraggo il campo anno dal json
    string anno = data["anno"];

    //cout << anno << endl;

    // Crea una connessione al database mysql
    sql::Connection* conn = creaConnessioneDB();

    // Scrivi la query sql come una stringa
    string query = "SELECT MONTH(dataAcquisto) AS mese, SUM(importo) AS totale "
        "FROM prenotazioni "
        "WHERE YEAR(dataAcquisto) = " + anno +
        " GROUP BY MONTH(dataAcquisto)";

    // Crea uno statement ed esegue la query, ottenendo un oggetto result
    sql::ResultSet* result = ottieni_risultato_query(conn, query);

    // Crea una stringa con i risultati separati da spazi e a capo
    string response;
    while (result->next()) {
        response += to_string(result->getInt("mese")) + " " + to_string(result->getInt("totale")) + "\n";
    }

    // Imposta il corpo della risposta con la stringa creata
    res.set_content(response, "text/plain");

    // Libera le risorse
    libera_risorse(result, conn);
    //delete stmt;
}
