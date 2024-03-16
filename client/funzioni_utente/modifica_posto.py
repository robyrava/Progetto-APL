from utilities import contatta_server
from config import path_richiesta_posto
from config import path_assegna_posto
import json
from funzioni_validazione import verifica_codice_prenotazione, verifica_documento, verifica_posto, validazione_input

def stampa_posti(posti):
    print("\n***POSTI DISPONIBILI***\n")
    # Sostituisce i valori "true" con "X" e "false" con il posto
    posti_formattati = {k: "X " if v == "true" else k for k, v in posti.items()}

    # Formatta la stringa per la stampa
    output = ""
    for i, posto in enumerate(posti_formattati.values(), start=1):
        output += posto
        if i % 3 != 0:
            output += "-"
        elif i % 6 == 0:
            output += "\n"
        else:
            output += "       "
    
    print(output)

def modifica_posto():
    print("\n***MODIFICA POSTO ASSEGNATO***\n")
    
    codice_prenotazione = validazione_input("Inserisci il codice prenotazione: ", verifica_codice_prenotazione, "codice prenotazione non valido")
    documento = validazione_input("Inserisci il documento: ", verifica_documento, "documento non valido")

    dati = {
        "documento": documento.upper(),
        "codicePrenotazione": codice_prenotazione,
    }

    #Invio la richiesta di modifica del posto
    esito,risposta = contatta_server(path_richiesta_posto, json.dumps(dati))

    if esito:
        elenco_posti = json.loads(risposta)

        #Stampo i posti disponibili
        stampa_posti(elenco_posti)

        #Validazione del posto
        posto = validazione_input("Inserisci il posto: ", lambda x: verifica_posto(x, elenco_posti), "Posto non valido o già occupato. Riprova.")
        
        dati = {
                "documento": documento.upper(),
                "codicePrenotazione": codice_prenotazione,
                "posto": posto.upper(),
            }

        #Invio la richiesta di modifica del posto
        _,risposta = contatta_server(path_assegna_posto, json.dumps(dati))
        
        #stampo la risposta ricevuta
        print(f"\n{risposta}")




    else:
        print(f"\n{risposta}")



