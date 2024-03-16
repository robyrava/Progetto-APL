#ifndef UTILITIES_H
#define UTILITIES_H

#include <mysql_connection.h> 

using namespace std;


// Funzione per creare una connessione al database
sql::Connection* creaConnessioneDB();


// Crea uno statement ed esegue la query, ottenendo un oggetto result
sql::ResultSet* ottieni_risultato_query(sql::Connection* conn, const string& query);


// Funzione per eseguire una query e restituire i risultati come stringa
string eseguiQuery(sql::Connection* conn, const string& query);


// Funzione che realizza il delete di un result set e/o di una connessione SQL
void libera_risorse(sql::ResultSet* result = nullptr, sql::Connection* conn = nullptr);


#endif