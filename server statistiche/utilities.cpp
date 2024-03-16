#include <iostream>
#include <mysql_driver.h> 
#include <mysql_connection.h> 
#include <cppconn/resultset.h>
#include <cppconn/statement.h>
#include "utilities.h"


using namespace std;


// Funzione per creare una connessione al database
sql::Connection* creaConnessioneDB() {
    sql::mysql::MySQL_Driver* driver;
    sql::Connection* conn;

    driver = sql::mysql::get_mysql_driver_instance();
    conn = driver->connect("tcp://127.0.0.1:3306/airline_manager", "root", "");

    return conn;
}

// Crea uno statement ed esegue la query, ottenendo un oggetto result
sql::ResultSet* ottieni_risultato_query(sql::Connection* conn, const string& query) {

    // Crea uno statement
    sql::Statement * stmt = conn->createStatement();

    // Esegui la query e ottieni un oggetto result
    sql::ResultSet* result = stmt->executeQuery(query);

    delete stmt;

    return result;

}

// Funzione per eseguire una query e restituire i risultati come stringa
string eseguiQuery(sql::Connection* conn, const string& query) {

    // Crea uno statement ed esegue la query, ottenendo un oggetto result
    sql::ResultSet* result = ottieni_risultato_query(conn, query);

    // Crea una stringa con i risultati separati da spazi e a capo
    string response;
    while (result->next()) {
        response += result->getString(1) + " " + to_string(result->getInt(2)) + "\n";
    }

    // Libera le risorse
    libera_risorse(result);
    
    return response;
}


// Funzione che realizza il delete di un result set e/o di una connessione SQL
void libera_risorse(sql::ResultSet* result, sql::Connection* conn) {

    if (result != nullptr) {
        delete result;
    }

    if (conn != nullptr) {
        delete conn;
    }
}