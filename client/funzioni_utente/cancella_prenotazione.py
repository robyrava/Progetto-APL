from funzioni_validazione import verifica_codice_prenotazione, verifica_documento, validazione_input
from config import path_cancella_prenotazione as path
from utilities import simula_tempo_attesa, contatta_server
import json


def cancella_prenotazione():
    print("\n\n*** CANCELLA PRENOTAZIONE ***\n")

    print("Inserisci i dati richiesti:\n")

    codice_prenotazione = validazione_input("Codice prenotazione: (12 caratteri alfanumerici) ", verifica_codice_prenotazione, "Inserisci un codice prenotazione valido (composto da 12 caratteri alfanumerici).\n")
    documento = validazione_input("Documento: (esempio: AY2899143) ", verifica_documento, "Inserisci un documento valido (composto da 2 lettere alfabetiche e 7 cifre)\n")

    stringa_1 = "\nI dati inseriti sono validi. Airline Manager sta elaborando la tua richiesta di cancellazione..."
    simula_tempo_attesa(stringa_1)

    dati = {
        "documento": documento,
        "codicePrenotazione": codice_prenotazione,
    }

    # invio la richiesta al server
    _, risposta = contatta_server(path, json.dumps(dati))

    print("\n" + risposta)