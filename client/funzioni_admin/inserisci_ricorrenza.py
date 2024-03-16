from config import path_inserisci_ricorrenza as path
from utilities import contatta_server
import json
from funzioni_validazione import verifica_codice_volo, confronta_data,validazione_input

def inserisci_ricorrenza():
    print("\n***INSERISCI RICORRENZA***\n")
    
    codice_volo = validazione_input("Inserisci il codice del volo (es: CT1234): ", verifica_codice_volo, "codice volo non valido")
    data_partenza = validazione_input("Inserisci la data di partenza (formato AAAA-MM-GG): ", lambda data: confronta_data(data1=data, boolean=True), "Data inserita non valida, deve essere maggiore o uguale alla data odierna")
    data_arrivo = validazione_input("Inserisci la data di arrivo (formato AAAA-MM-GG): ", lambda data: confronta_data(data, data_partenza, True), "Data inserita non valida, deve essere maggiore o uguale alla data di partenza")

    # creo dizionario con i dati della ricorrenza (numero posti occupati e posti verrà inizializzato dal server)
    ricorrenza = { 
        "CodiceVolo": codice_volo.upper(),
        "DataPartenza": data_partenza,
        "DataArrivo": data_arrivo,
    }

    # invio la richiesta al server
    _, risposta = contatta_server(path, json.dumps(ricorrenza))

    print("\n" + risposta)
