from config import path_modifica_ricorrenza as path
import json
from funzioni_validazione import verifica_codice_volo, verifica_data, confronta_data,validazione_input
from utilities import contatta_server

def modifica_data_ricorrenza():
    codice_volo = validazione_input("Inserisci il codice del volo (es: CT1234): ", verifica_codice_volo, "codice volo non valido")
    data_partenza = validazione_input("Inserisci la data di partenza (formato AAAA-MM-GG): ", lambda data: confronta_data(data1=data, boolean=True), "Data inserita non valida, deve essere maggiore o uguale alla data odierna")
    data_arrivo = validazione_input("Inserisci la data di arrivo (formato AAAA-MM-GG): ", lambda data: confronta_data(data, data_partenza, True), "Data inserita non valida, deve essere maggiore o uguale alla data di partenza")
    nuova_data_partenza = validazione_input("Inserisci la nuova data di partenza (formato AAAA-MM-GG): ", verifica_data, "Data inserita non valida")
    nuova_data_arrivo = validazione_input("Inserisci la nuova data di arrivo (formato AAAA-MM-GG): ", lambda data: confronta_data(data, nuova_data_partenza, True), "Data inserita non valida.")

    # creo dizionario con i dati della ricorrenza
    ricorrenza = { 
        "codiceVolo": codice_volo.upper(),
        "dataPartenza": data_partenza,
        "dataArrivo": data_arrivo,
        "nuovaDataPartenza": nuova_data_partenza,
        "nuovaDataArrivo": nuova_data_arrivo,
    }

    # invio la richiesta al server
    _, risposta = contatta_server(path, json.dumps(ricorrenza))

    print("\n" + risposta)
