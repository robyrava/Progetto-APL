from funzioni_validazione import verifica_codice_volo, validazione_input
from utilities import contatta_server
from config import path_statistica_due as path
import json
import matplotlib.pyplot as plt


# Statistica 2: percentuale dei posti venduti rispetto al totale, per ciascuna ricorrenza di un volo

def statistica_2():

    # Inserimento e validazione codice volo
    codice_volo = validazione_input("Inserisci il codice del volo (es: AS1234): ", verifica_codice_volo, "Il codice volo inserito non è valido. Riprova.")

    # dizionario
    dati = {"codiceVolo": codice_volo}

    # Invio dati al server
    ok, risposta = contatta_server(path, json.dumps(dati), True)

    #print(risposta)

    # verifico esito della richiesta al server statistiche
    if not ok:
        print("\nNessun dato disponibile")
        return

    # Converto il risultato in una lista di tuple (date partenza, valori percentuali)
    result = [tuple(map(str, row.split())) for row in risposta.split("\n") if row]

    # Estraggo elenco dei mesi e dei corrispondenti valori di guadagno in due liste separate
    date_partenza = [row[0] for row in result]
    valori_percentuali = [row[1] for row in result]

    # Genero istogramma con: mesi in asse x, valori di guadagno in asse y
    plt.bar(date_partenza, valori_percentuali, edgecolor="black", facecolor="red", width=0.3)

    #plt.xticks([0,1,2,3,4])

    # assegno titolo al grafico
    plt.title("Volo " + codice_volo + ": percentuale di posti occupati per ricorrenza")

    # assegno titoli agli assi
    plt.xlabel("Data di partenza", weight="bold", color="black")
    plt.ylabel("% posti occupati", weight="bold", color="black")

    # Mostra il grafico
    plt.show()