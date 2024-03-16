import json
from funzioni_validazione import verifica_codice_volo,validazione_input, verifica_data
from config import path_elimina_ricorrenza as path
from utilities import contatta_server


def elimina_ricorrenza():
    print("\n***Elimina ricorrenza***\n")
    
    codice_volo = validazione_input("Inserisci il codice del volo (es: CT1234): ", verifica_codice_volo, "codice volo non valido")
    data_partenza = validazione_input("Inserisci la data di partenza (formato AAAA-MM-GG): ", verifica_data, "Data inserita non valida")
    data_arrivo = validazione_input("Inserisci la data di arrivo (formato AAAA-MM-GG): ", verifica_data, "Data inserita non valida")
    
    # creo dizionario con i dati della ricorrenza
    ricorrenza = { 
        "codiceVolo": codice_volo.upper(),
        "dataPartenza": data_partenza,
        "dataArrivo": data_arrivo,
    }

    _, risposta = contatta_server(path, json.dumps(ricorrenza))

    print("\n" + risposta)
