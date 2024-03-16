from funzioni_validazione import verifica_anno, validazione_input
from utilities import contatta_server, genera_labels_mesi
from config import path_statistica_quattro as path
import json
import matplotlib.pyplot as plt


# Statistica 4: guadagno mensile della compagnia, relativamente ad uno specifico anno

def statistica_4():

    #  Inserimento e validazione anno
    anno = validazione_input("Inserisci l'anno: ", verifica_anno, "L'anno inserito non è valido. Riprova.")

    # dizionario
    dati = {"anno": anno}

    # Invio dati al server
    ok, risposta = contatta_server(path, json.dumps(dati), True)

    #print(risposta)

    if not ok:
        print("\nNessun dato disponibile")
        return

    # Converto il risultato in una lista di tuple (mesi, valori di guadagno)
    result = [tuple(map(int, row.split())) for row in risposta.split("\n") if row]

    # Estraggo elenco dei mesi e dei corrispondenti valori di guadagno in due liste separate
    mesi = [row[0] for row in result]
    valori_guadagno = [row[1] for row in result]

    # Genero istogramma con: mesi in asse x, valori di guadagno in asse y
    plt.bar(mesi, valori_guadagno, edgecolor="black", facecolor="green", width=0.4)
    #edgecolor = "black"

    lista_mesi = genera_labels_mesi(mesi)
    # valori stampati asse x
    plt.xticks(mesi, lista_mesi)

    # titolo grafico
    plt.title("Guadagno mensile della compagnia nel " + anno)

    # titolo negli assi
    plt.xlabel("Mese", weight="bold", color="black")
    plt.ylabel("Guadagno", weight="bold", color="black")

    # Mostra il grafico
    plt.show()