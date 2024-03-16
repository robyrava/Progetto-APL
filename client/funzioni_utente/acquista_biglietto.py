from funzioni_validazione import *
from config import path_ricerca_voli_acquisto, path_inserisci_prenotazione, path_inserisci_passeggero, path_modifica_ricorrenza_numero_posti_occupati
from utilities import stampa_menu, contatta_server, stampa_voli_trovati, simula_tempo_attesa, \
    genera_dizionario_passeggero, stampa_riepilogo_prenotazione
import json


def routine_acquista_biglietto():
    print("\n\n*** ACQUISTA BIGLIETTO ***\n")

    opzione = 0
    while opzione != 4:
        stampa_menu(opzioni_acquisto)
        opzione = input("Scelta: ")
        try:
            opzione = int(opzione)
            if opzione == 1 or opzione == 2:
                acquista_biglietto(opzione)
            elif opzione == 3:
                from funzioni_utente.utente import routine_utente
                routine_utente()
            elif opzione == 4:
                from main import main
                main()
            elif opzione == 5:
                print("\nGrazie per aver utilizzato Airline Manager! Arrivederci!")
                exit()
            else:
                raise ValueError
        except ValueError:
            print("\nHai inserito un'opzione non valida. Riprova.")


def acquista_biglietto(opzione):
    (esito, data_partenza, risultati) = ricerca_voli_data_iata(opzione)

    if not esito:
        print("\n" + risultati)
        print("Grazie per aver utilizzato Airline Manager!")
        from funzioni_utente.utente import routine_utente
        routine_utente()
    else:
        risultati = json.loads(risultati)
        counter = stampa_voli_trovati(risultati)

    volo_selezionato = input("Volo selezionato: ")
    try:
        volo_selezionato = int(volo_selezionato)
    except:
        print("\nHai inserito un valore non valido. Riavvio sessione acquisto.\n")  # messaggio di errore
        routine_acquista_biglietto()

    print("Hai selezionato il seguente volo:\n", volo_selezionato)

    if volo_selezionato > counter:
        print("\nIl volo selezionato non è presente in elenco. Riavvio sessione acquisto.\n")  # messaggio di errore
        routine_acquista_biglietto()

    codice_volo = risultati[volo_selezionato - 1]['codiceVolo']
    importo = risultati[volo_selezionato - 1]['prezzoBase']

    print("\nInserisci informazioni da associare alla prenotazione: ")
    (documento, codice_fiscale, nome, cognome, data_nascita, telefono) = acquisisci_dati_passeggero()

    dati = {
        "documento": documento,
        "codiceVolo": codice_volo,
        "dataPartenza": data_partenza,
        "importo": importo
    }

    # chiamata al server (inserisci nuova prenotazione)
    (esito, codice_prenotazione) = contatta_server(path_inserisci_prenotazione, json.dumps(dati))

    if not esito:
        print(codice_prenotazione)
    else:
        dati = genera_dizionario_passeggero(documento, codice_prenotazione, codice_fiscale, nome, cognome, data_nascita, telefono)

        # chiamata al server (inserisci nuovo passeggero)
        (esito, risposta) = contatta_server(path_inserisci_passeggero, json.dumps(dati))

        if not esito:
            print(risposta)
        else:
            dati = {
                "codiceVolo": codice_volo,
                "dataPartenza": data_partenza
            }

            # chiamata al server (aggiorna ricorrenza)
            (esito, risposta) = contatta_server(path_modifica_ricorrenza_numero_posti_occupati, json.dumps(dati))

            if not esito:
                print(risposta)
            else:
                simula_tempo_attesa(output[0], output[1])
                stampa_riepilogo_prenotazione(codice_prenotazione, risultati[volo_selezionato-1])


def ricerca_voli_data_iata(opzione):

    print("\nSpecifica parametri di ricerca:\n")

    # input e validazione
    data_partenza = validazione_input("Data di partenza: (formato: aaaa-mm-gg) ", lambda data: confronta_data(data, boolean=True), "Inserisci una data di partenza valida (non dev'essere precedente a quella odierna).\n")
    iata_partenza = validazione_input("Aeroporto di partenza: (codice IATA) ", verifica_iata, "Inserisci un aeroporto di partenza valido (composto da 3 lettere)\n")

    dati = {
        "dataPartenza": data_partenza,
        "iataAeroportoPartenza": iata_partenza
    }

    if(opzione == 1): # utente vuole cercare voli sulla base di: data partenza, iata partenza e arrivo

        # input e validazione aeroporto arrivo
        iata_arrivo = validazione_input("Aeroporto di arrivo: (codice IATA) ", lambda iata: verifica_iata(iata) and verifica_iata_diversi(iata_partenza, iata), "Inserisci un aeroporto di arrivo valido (composto da 3 lettere e diverso da quello di partenza)\n")

        dati['iataAeroportoArrivo'] = iata_arrivo

        # chiamata al server
        (volo_trovato, risultati) = contatta_server(path_ricerca_voli_acquisto, json.dumps(dati))

    else:  # utente vuole cercare voli sulla base di: data partenza, iata partenza (NO iata arrivo)
        # chiamata al server
        (volo_trovato, risultati) = contatta_server(path_ricerca_voli_acquisto, json.dumps(dati))

    return volo_trovato, data_partenza, risultati


def acquisisci_dati_passeggero():

    documento = validazione_input("Documento: (esempio: AY2899143) ", verifica_documento, "Inserisci un documento valido (composto da 2 lettere alfabetiche e 7 cifre)\n")
    codice_fiscale = validazione_input("Codice fiscale: (esempio: MGLRBT94P12C352F) ", verifica_codice_fiscale, "Inserisci un codice fiscale valido\n")
    nome = validazione_input("Nome: (esempio: Mario) ", verifica_nome_cognome, "Inserisci un nome valido (composto da sole lettere alfabetiche, min 1, max 20)\n")
    cognome = validazione_input("Cognome: (esempio: Rossi) ", verifica_nome_cognome, "Inserisci un cognome valido (composto da sole lettere alfabetiche, min 1, max 20)\n")
    data_nascita = validazione_input("Data di nascita: (formato: aaaa-mm-gg) ", lambda data: confronta_data(data), "Inserisci una data di nascita valida (dev'essere precedente a quella odierna)\n")
    telefono = validazione_input("Telefono: ", verifica_telefono, "Inserisci un numero di telefono valido (composto da 10 cifre)\n")


    return documento, codice_fiscale, nome, cognome, data_nascita, telefono




opzioni_acquisto = [
                     "1. Ricerca voli inserendo: data partenza, iata aeroporto partenza, iata aeroporto arrivo",
                     "2. Ricerca voli inserendo: data partenza, iata aeroporto partenza",
                     "3. Torna al menu utente",
                     "4. Torna al menu principale",
                     "5. Esci\n"
                   ]

output = [
            "\nI dati inseriti sono validi. Airline Manager sta elaborando il tuo acquisto...",
            "Il tuo acquisto è stato elaborato con successo!"
         ]