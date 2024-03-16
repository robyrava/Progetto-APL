import json
from funzioni_validazione import verifica_codice_volo,validazione_input
from config import path_ricerca_volo as path
from utilities import contatta_server

def stampa_ricorrenze_volo(json_data):
    # Estrai i valori dei campi dalla prima riga del json
    aeroporto_partenza = json_data[0]["iataAeroportoPartenza"]
    aeroporto_arrivo = json_data[0]["iataAeroportoArrivo"]
                
    # Stampa il titolo con i valori dei campi
    print(f"\nVoli {aeroporto_partenza} -> {aeroporto_arrivo} schedulati:")
    # Inizializza un contatore per il numero di volo
    count = 1
    # Itera sulla lista di dizionari
    for flight in json_data:
        # Estrae le informazioni dal dizionario
        data_partenza = flight["dataPartenza"][:10] # Prende solo la data, non l'ora
        data_arrivo = flight["dataArrivo"][:10]
        posti_occupati = flight["numeroPostiOccupati"]
        prezzo = flight["prezzoBase"]
        # Formatta e stampa la riga con le informazioni
        print(f"\n{count}. Partenza: {data_partenza} Arrivo: {data_arrivo} Posti Occupati = {posti_occupati} Prezzo Base = {prezzo}€")
        
        count += 1


def visualizza_volo():
    print("\n***Visualizza volo***\n")
    
    codice_volo = validazione_input("Inserisci il codice del volo (es: CT1234): ", verifica_codice_volo, "codice volo non valido")

    esito, dati = contatta_server(path, json.dumps(codice_volo.upper()))

    if esito:
            stampa_ricorrenze_volo(json.loads(dati))
    else:
        print("\n" + dati)

               
         