from utilities import contatta_server
from config import path_modifica_bagaglio as path
import json
from funzioni_validazione import verifica_codice_prenotazione, verifica_documento, validazione_input

def aggiungi_bagaglio():
    # Richiesta dati
    codice_prenotazione = validazione_input("Inserisci il codice prenotazione: ", verifica_codice_prenotazione, "codice prenotazione non valido")
    documento = validazione_input("Inserisci il documento: ", verifica_documento, "documento non valido")
    
    dati = {
        "documento": documento.upper(),
        "codicePrenotazione": codice_prenotazione,
    }

    #Invio la richiesta al server
    _,risposta = contatta_server(path, json.dumps(dati))

    print("\n" + risposta)
