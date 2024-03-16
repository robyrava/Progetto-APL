import json
from funzioni_validazione import verifica_codice_volo, verifica_iata, verifica_prezzo_base,validazione_input, verifica_iata_diversi
from config import path_inserisci_volo as path
from utilities import contatta_server

def inserisci_volo():
    # Inserimento del volo da parte dell'admin

    codice_volo = validazione_input("Inserisci il codice del volo (es: CT1234): ", verifica_codice_volo, "codice volo non valido")
    iata_aeroporto_partenza = validazione_input("Inserisci l'IATA dell'aeroporto di partenza: ", verifica_iata, "L'IATA deve essere composta da 3 caratteri.")
    iata_aeroporto_arrivo = validazione_input("Inserisci l'IATA dell'aeroporto di arrivo: ", lambda iata: verifica_iata(iata) and verifica_iata_diversi(iata_aeroporto_partenza, iata), "L'IATA deve essere composta da 3 caratteri e deve essere diversa dall'aeroporto di partenza.")
    prezzo_base = validazione_input("Inserisci il prezzo base del volo: ", verifica_prezzo_base, "Inserisci un prezzo valido (ES: 50,50)")

    # creo dizionario con i dati del volo
    volo = { 
        "CodiceVolo": codice_volo.upper(),
        "IataAeroportoPartenza": iata_aeroporto_partenza.upper(),
        "IataAeroportoArrivo": iata_aeroporto_arrivo.upper(),
        "PrezzoBase": prezzo_base
    }

    # Invio i dati al server
    _, risposta = contatta_server(path, json.dumps(volo))
    
    print("\n" + risposta)

    
