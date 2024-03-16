from funzioni_validazione import verifica_codice_prenotazione, verifica_documento, validazione_input
from config import path_check_in as path
from utilities import simula_tempo_attesa, contatta_server, stampa_carta_imbarco
import json


def check_in():
    print("\n\n*** EFFETTUA CHECK-IN ***\n")

    print("Inserisci i dati richiesti:\n")

    codice_prenotazione = validazione_input("Codice prenotazione: (12 caratteri alfanumerici) ", verifica_codice_prenotazione, "Inserisci un codice prenotazione valido (composto da 12 caratteri alfanumerici).\n")
    documento = validazione_input("Documento: (esempio: AY2899143) ", verifica_documento, "Inserisci un documento valido (composto da 2 lettere alfabetiche e 7 cifre)\n")


    stringa_1 = "\nI dati inseriti sono validi. Airline Manager sta elaborando la tua richiesta di check-in..."
    simula_tempo_attesa(stringa_1)

    dati = {
        "documento": documento,
        "codicePrenotazione": codice_prenotazione,
    }

    # invio la richiesta al server
    esito, risposta = contatta_server(path, json.dumps(dati))

    if not esito:
        print("\n" + risposta)
        print("Grazie per aver usato Airline Manager!\n")
    else:
        stampa_carta_imbarco(json.loads(risposta))